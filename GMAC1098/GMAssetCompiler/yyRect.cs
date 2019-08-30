using System.Collections.Generic;
using System.Drawing;

namespace GMAssetCompiler
{
	public class yyRect
	{
		public ulong CRC;

		public bool packflag;

		public bool full;

		public ushort x;

		public ushort y;

		public ushort width;

		public ushort height;

		public int arealeft;

		public List<yyRect> Children;

		public int top
		{
			get
			{
				return y;
			}
		}

		public int left
		{
			get
			{
				return x;
			}
		}

		public int bottom
		{
			get
			{
				return y + height;
			}
		}

		public int right
		{
			get
			{
				return x + width;
			}
		}

		public int area
		{
			get
			{
				return width * height;
			}
		}

		public override string ToString()
		{
			return string.Format("{0},{1}, {2},{3},  {4}", x, y, width, height, area);
		}

		public static yyRect Create(int _width, int _height)
		{
			yyRect yyRect = new yyRect(0, 0, _width, _height);
			yyRect.Children = new List<yyRect>();
			yyRect.Children.Add(yyRect);
			return yyRect;
		}

		public yyRect(int _x, int _y, int _width, int _height)
		{
			Children = null;
			x = (ushort)_x;
			y = (ushort)_y;
			width = (ushort)_width;
			height = (ushort)_height;
			arealeft = area;
			CRC = (x | ((ulong)y << 16) | ((ulong)width << 32) | ((ulong)height << 48));
		}

		private List<yyRect> Clip(int _x, int _y, int _w, int _h, out bool _full)
		{
			_full = false;
			if (_x >= right || _x + _w <= left || _y >= bottom || _y + _h <= top)
			{
				return null;
			}
			if (right > _x && left < _x + _w && top > _y && bottom < _y + height)
			{
				_full = true;
				return null;
			}
			if (_x == x && _y == y && _w == width && _h == height)
			{
				_full = true;
				return null;
			}
			List<yyRect> list = new List<yyRect>();
			if (_x > left)
			{
				int num = _x - left;
				list.Add(new yyRect(left, y, num, height));
			}
			int num2 = _x + _w;
			if (num2 < right)
			{
				list.Add(new yyRect(num2, y, right - num2, height));
			}
			if (_y > top)
			{
				int num3 = _y - top;
				list.Add(new yyRect(x, y, width, num3));
			}
			int num4 = _y + _h;
			if (num4 < bottom)
			{
				list.Add(new yyRect(x, num4, width, bottom - num4));
			}
			return list;
		}

		public yyRect Test(int _width, int _height)
		{
			arealeft = 0;
			if (Children.Count == 0)
			{
				return null;
			}
			yyRect result = null;
			foreach (yyRect child in Children)
			{
				foreach (yyRect child2 in Children)
				{
					bool _full = false;
					if (child2.width >= _width && child2.height >= _height)
					{
						List<yyRect> list = child.Clip(child2.x, child2.y, _width, _height, out _full);
						if (list == null && _full)
						{
							arealeft = child.arealeft;
							result = child2;
						}
						else if (list != null && list.Count != 0)
						{
							foreach (yyRect item in list)
							{
								if (item != null && arealeft < item.arealeft)
								{
									arealeft = item.arealeft;
									result = child2;
								}
							}
						}
						else if (arealeft < child.arealeft)
						{
							arealeft = child.arealeft;
							result = child2;
						}
					}
				}
			}
			return result;
		}

		private List<yyRect> AddImageEx(int _width, int _height, out bool _full)
		{
			_full = false;
			List<yyRect> list = Clip(x, y, _width, _height, out _full);
			if (list == null || list.Count == 0)
			{
				return null;
			}
			return list;
		}

		private void AddRange(List<yyRect> _rects)
		{
			foreach (yyRect _rect in _rects)
			{
				bool flag = false;
				int count = Children.Count;
				for (int i = 0; i < count; i++)
				{
					if (Children[i].CRC == _rect.CRC)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					Children.Add(_rect);
				}
			}
		}

		public void CompactList()
		{
			foreach (yyRect child in Children)
			{
				child.packflag = false;
			}
			List<yyRect> list = new List<yyRect>(128);
			foreach (yyRect child2 in Children)
			{
				if (!child2.packflag)
				{
					foreach (yyRect child3 in Children)
					{
						if (child2 != child3 && !child3.packflag && child3.x >= child2.x && child3.y >= child2.y && child3.right <= child2.right && child3.bottom <= child2.bottom)
						{
							list.Add(child3);
							child3.packflag = true;
						}
					}
				}
			}
			foreach (yyRect item in list)
			{
				Children.Remove(item);
			}
		}

		public Point AddImage(yyRect _rect, int _width, int _height)
		{
			if (_rect == null)
			{
				return new Point(-1, -1);
			}
			bool _full = false;
			List<yyRect> list = _rect.AddImageEx(_width, _height, out _full);
			int num = _rect.x;
			int num2 = _rect.y;
			if (_full || (list != null && list.Count != 0))
			{
				if (_rect != this)
				{
					Children.Remove(_rect);
				}
				if (list != null && list.Count != 0)
				{
					if (Children == null)
					{
						Children = new List<yyRect>(64);
					}
					AddRange(list);
				}
			}
			List<yyRect> list2 = new List<yyRect>(64);
			List<yyRect> list3 = new List<yyRect>(64);
			foreach (yyRect child in Children)
			{
				_full = false;
				list = child.Clip(num, num2, _width, _height, out _full);
				if (_full || (list != null && list.Count != 0))
				{
					list2.Add(child);
					if (list != null && list.Count != 0)
					{
						list3.AddRange(list);
					}
				}
			}
			foreach (yyRect item in list2)
			{
				Children.Remove(item);
			}
			if (list3.Count != 0)
			{
				AddRange(list3);
			}
			CompactList();
			return new Point(num, num2);
		}

		public void Draw(Graphics _gr, int _ox, int _oy)
		{
			if (Children != null && Children.Count != 0)
			{
				foreach (yyRect child in Children)
				{
					_gr.DrawRectangle(rect: new Rectangle(child.x + _ox, child.y + _oy, child.width - 1, child.height - 1), pen: Pens.Yellow);
				}
			}
		}
	}
}
