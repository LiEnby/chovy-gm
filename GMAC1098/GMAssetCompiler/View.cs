using Flobbster.Windows.Forms;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using System.Runtime.InteropServices;

namespace GMAssetCompiler
{
	public class View<T> : IPropertyGrid
	{
		protected T m_this;

		protected PropertyInfo[] m_props;

		public View(T _entry)
		{
			m_this = _entry;
			Type typeFromHandle = typeof(T);
			m_props = typeFromHandle.GetProperties();
		}

		public PropertyBag Prepare()
		{
			PropertyBag propertyBag = new PropertyBag();
			propertyBag.GetValue += GetValue;
			PropertyInfo[] props = m_props;
			foreach (PropertyInfo propertyInfo in props)
			{
				string name = propertyInfo.Name;
				string description = string.Empty;
				string category = "default";
				object defaultValue = null;
				object[] customAttributes = propertyInfo.GetCustomAttributes(true);
				PropertyAttribute propertyAttribute = null;
				object[] array = customAttributes;
				foreach (object obj in array)
				{
					propertyAttribute = (obj as PropertyAttribute);
					if (propertyAttribute != null)
					{
						description = propertyAttribute.Description;
						if (!string.IsNullOrEmpty(propertyAttribute.Category))
						{
							category = propertyAttribute.Category;
						}
						defaultValue = propertyAttribute.Default;
						if (!string.IsNullOrEmpty(propertyAttribute.Name))
						{
							name = propertyAttribute.Name;
						}
						break;
					}
				}
				if (propertyAttribute == null || !propertyAttribute.Disabled)
				{
					PropertySpec value = new PropertySpec(name, propertyInfo.PropertyType, category, description, defaultValue);
					propertyBag.Properties.Add(value);
				}
			}
			return propertyBag;
		}

		private void GetValue(object _sender, PropertySpecEventArgs _e)
		{
			PropertyInfo[] props = m_props;
			int num = 0;
			PropertyInfo propertyInfo;
			while (true)
			{
				if (num < props.Length)
				{
					propertyInfo = props[num];
					if (propertyInfo.Name == _e.Property.Name)
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			_e.Value = propertyInfo.GetGetMethod().Invoke(m_this, null);
		}

		public virtual Image PrepareImage()
		{
			return null;
		}

		protected void CopyBits(BitmapData _data, int _x, int _y, GMBitmap32 _bmp)
		{
			IntPtr intPtr = new IntPtr(_data.Scan0.ToInt64() + _y * _data.Stride + _x * 4);
			int num = 0;
			int num2 = 0;
			while (num2 < _bmp.Height)
			{
				int num3 = num;
				IntPtr ptr = intPtr;
				int num4 = 0;
				while (num4 < _bmp.Width)
				{
					int val = _bmp.Data[num3] + (_bmp.Data[num3 + 1] << 8) + (_bmp.Data[num3 + 2] << 16) + ((0xFF | _bmp.Data[num3 + 3]) << 24);
					Marshal.WriteInt32(ptr, val);
					num4++;
					num3 += 4;
					ptr = new IntPtr(ptr.ToInt64() + 4);
				}
				num2++;
				intPtr = new IntPtr(intPtr.ToInt64() + _data.Stride);
				num += _bmp.Width * 4;
			}
		}

		protected void CopyBits(BitmapData _data, int _x, int _y, byte[] _mask, int _width, int _height)
		{
			IntPtr intPtr = new IntPtr(_data.Scan0.ToInt64() + _y * _data.Stride + _x * 4);
			int num = (_width + 7) / 8;
			int num2 = 0;
			int num3 = 0;
			while (num3 < _height)
			{
				int num4 = num2;
				IntPtr ptr = intPtr;
				byte b = 128;
				int num5 = 0;
				while (num5 < _width)
				{
					if (b == 0)
					{
						b = 128;
						num4++;
					}
					int val = 16777215;
					if ((_mask[num4] & b) != 0)
					{
						val = -16777216;
					}
					Marshal.WriteInt32(ptr, val);
					num5++;
					b = (byte)(b >> 1);
					ptr = new IntPtr(ptr.ToInt64() + 4);
				}
				num3++;
				intPtr = new IntPtr(intPtr.ToInt64() + _data.Stride);
				num2 += num;
			}
		}
	}
}
