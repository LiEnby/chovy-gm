using System;
using System.Collections.Generic;

namespace GMAssetCompiler
{
	public class Lex
	{
		private const int MAX_QUEUE = 100;

		private bool m_CaseSensitive;

		private string m_Text;

		private int m_Index;

		private eLex[] m_TokenLookup = new eLex[256];

		private eLex[] m_IDLookup = new eLex[256];

		private int[] m_HexLookup = new int[256];

		private int[] m_BinLookup = new int[256];

		private int[] m_DecLookup = new int[256];

		private eLex[] m_SymbolLookup1 = new eLex[256];

		private eLex[] m_SymbolLookup2 = new eLex[256];

		private eLex[] m_LexToken = new eLex[100];

		private string[] m_LexText = new string[100];

		private double[] m_LexValue = new double[100];

		private int m_Head;

		private int m_Tail;

		private YYObfuscate m_pObfuscate;

		private Dictionary<string, eLex> m_CommandLookup;

		public string yyText
		{
			get
			{
				return m_LexText[m_Tail];
			}
		}

		public double yyValue
		{
			get
			{
				return m_LexValue[m_Tail];
			}
		}

		public eLex yyToken
		{
			get
			{
				return m_LexToken[m_Tail];
			}
		}

		public string Text
		{
			get
			{
				return m_Text;
			}
			set
			{
				m_Text = value;
				m_Index = 0;
				m_Tail = 0;
				m_Head = 0;
				if (!CaseSensitive)
				{
					m_Text = m_Text.ToLower();
				}
			}
		}

		public bool CaseSensitive
		{
			get
			{
				return m_CaseSensitive;
			}
			set
			{
				m_CaseSensitive = value;
			}
		}

		public void AddSymbol(string _s, eLex _token)
		{
			m_SymbolLookup1[_s[0]] = m_TokenLookup[_s[0]];
			m_SymbolLookup2[_s[1]] = _token;
		}

		public void AddCommand(string _cmd, eLex _token)
		{
			eLex value;
			if (!m_CommandLookup.TryGetValue(_cmd, out value))
			{
				m_CommandLookup.Add(_cmd, _token);
			}
		}

		public Lex(YYObfuscate _pObfuscate)
		{
			m_pObfuscate = _pObfuscate;
			CaseSensitive = false;
			m_TokenLookup[124] = eLex.Bar;
			m_TokenLookup[63] = eLex.QuestionMark;
			m_TokenLookup[33] = eLex.Exclamation;
			m_TokenLookup[34] = eLex.Quotes;
			m_TokenLookup[39] = eLex.SingleQuotes;
			m_TokenLookup[36] = eLex.Dollar;
			m_TokenLookup[37] = eLex.Percent;
			m_TokenLookup[94] = eLex.Carrot;
			m_TokenLookup[38] = eLex.Ampersand;
			m_TokenLookup[42] = eLex.Star;
			m_TokenLookup[40] = eLex.LeftBracket;
			m_TokenLookup[41] = eLex.RightBracket;
			m_TokenLookup[91] = eLex.LeftSquareBracket;
			m_TokenLookup[93] = eLex.RightSquareBracket;
			m_TokenLookup[45] = eLex.Minus;
			m_TokenLookup[43] = eLex.Plus;
			m_TokenLookup[61] = eLex.Equals;
			m_TokenLookup[47] = eLex.Divide;
			m_TokenLookup[44] = eLex.Comma;
			m_TokenLookup[58] = eLex.Colon;
			m_TokenLookup[59] = eLex.SemiColon;
			m_TokenLookup[123] = eLex.LCB;
			m_TokenLookup[125] = eLex.RCB;
			m_TokenLookup[60] = eLex.LessThan;
			m_TokenLookup[62] = eLex.GreaterThan;
			m_TokenLookup[46] = eLex.Dot;
			m_TokenLookup[126] = eLex.Not;
			m_TokenLookup[49] = eLex.Decimal;
			m_TokenLookup[50] = eLex.Decimal;
			m_TokenLookup[51] = eLex.Decimal;
			m_TokenLookup[52] = eLex.Decimal;
			m_TokenLookup[53] = eLex.Decimal;
			m_TokenLookup[54] = eLex.Decimal;
			m_TokenLookup[55] = eLex.Decimal;
			m_TokenLookup[56] = eLex.Decimal;
			m_TokenLookup[57] = eLex.Decimal;
			m_TokenLookup[48] = eLex.Decimal;
			m_TokenLookup[10] = eLex.NewLine;
			m_TokenLookup[13] = eLex.NewLine;
			m_TokenLookup[9] = eLex.WhiteSpace;
			m_TokenLookup[32] = eLex.WhiteSpace;
			AddSymbol("==", eLex.EqualEqual);
			AddSymbol("!=", eLex.NotEqual);
			AddSymbol("&=", eLex.AndEqual);
			AddSymbol("~=", eLex.EorEqual);
			AddSymbol("-=", eLex.MinusEqual);
			AddSymbol("+=", eLex.PlusEqual);
			AddSymbol("^=", eLex.XorEqual);
			AddSymbol("&&", eLex.AndAnd);
			AddSymbol("||", eLex.OrOr);
			AddSymbol("<<", eLex.ShiftLeft);
			AddSymbol(">>", eLex.ShiftRight);
			for (int i = 65; i <= 90; i++)
			{
				m_IDLookup[i] = eLex.ID;
			}
			for (int j = 97; j <= 122; j++)
			{
				m_IDLookup[j] = eLex.ID;
			}
			m_IDLookup[48] = eLex.ID;
			m_IDLookup[49] = eLex.ID;
			m_IDLookup[50] = eLex.ID;
			m_IDLookup[51] = eLex.ID;
			m_IDLookup[52] = eLex.ID;
			m_IDLookup[53] = eLex.ID;
			m_IDLookup[54] = eLex.ID;
			m_IDLookup[55] = eLex.ID;
			m_IDLookup[56] = eLex.ID;
			m_IDLookup[57] = eLex.ID;
			m_IDLookup[95] = eLex.ID;
			m_IDLookup[64] = eLex.ID;
			for (int num = m_HexLookup.Length - 1; num >= 0; num--)
			{
				m_HexLookup[num] = -1;
			}
			m_HexLookup[48] = 0;
			m_HexLookup[49] = 1;
			m_HexLookup[50] = 2;
			m_HexLookup[51] = 3;
			m_HexLookup[52] = 4;
			m_HexLookup[53] = 5;
			m_HexLookup[54] = 6;
			m_HexLookup[55] = 7;
			m_HexLookup[56] = 8;
			m_HexLookup[57] = 9;
			m_HexLookup[65] = 10;
			m_HexLookup[66] = 11;
			m_HexLookup[67] = 12;
			m_HexLookup[68] = 13;
			m_HexLookup[69] = 14;
			m_HexLookup[70] = 15;
			m_HexLookup[97] = 10;
			m_HexLookup[98] = 11;
			m_HexLookup[99] = 12;
			m_HexLookup[100] = 13;
			m_HexLookup[101] = 14;
			m_HexLookup[102] = 15;
			for (int num2 = m_BinLookup.Length - 1; num2 >= 0; num2--)
			{
				m_BinLookup[num2] = -1;
			}
			m_BinLookup[48] = 0;
			m_BinLookup[49] = 1;
			for (int num3 = m_DecLookup.Length - 1; num3 >= 0; num3--)
			{
				m_DecLookup[num3] = -1;
			}
			m_DecLookup[48] = 0;
			m_DecLookup[49] = 1;
			m_DecLookup[50] = 2;
			m_DecLookup[51] = 3;
			m_DecLookup[52] = 4;
			m_DecLookup[53] = 5;
			m_DecLookup[54] = 6;
			m_DecLookup[55] = 7;
			m_DecLookup[56] = 8;
			m_DecLookup[57] = 9;
			m_CommandLookup = new Dictionary<string, eLex>();
			AddCommand("Object", eLex.reserved);
			AddCommand("Function", eLex.reserved);
			AddCommand("defineProperty", eLex.reserved);
			AddCommand("apply", eLex.reserved);
			AddCommand("call", eLex.reserved);
			AddCommand("__defineGetter__", eLex.reserved);
			AddCommand("__defineSetter__", eLex.reserved);
			AddCommand("__implements", eLex.reserved);
			AddCommand("__super", eLex.reserved);
			AddCommand("function", eLex.reserved2);
			AddCommand("return", eLex.reserved2);
			AddCommand("typeof", eLex.reserved2);
			AddCommand("instanceof", eLex.reserved3);
			AddCommand("continue", eLex.reserved);
			AddCommand("for", eLex.reserved);
			AddCommand("this", eLex.reserved);
			AddCommand("var", eLex.reserved2);
			AddCommand("true", eLex.reserved);
			AddCommand("false", eLex.reserved);
			AddCommand("null", eLex.reserved);
			AddCommand("new", eLex.reserved2);
			AddCommand("delete", eLex.reserved2);
			AddCommand("with", eLex.reserved);
			AddCommand("if", eLex.reserved2);
			AddCommand("else", eLex.reserved2);
			AddCommand("switch", eLex.reserved2);
			AddCommand("case", eLex.reserved2);
			AddCommand("default", eLex.reserved2);
			AddCommand("break", eLex.reserved);
			AddCommand("try", eLex.reserved);
			AddCommand("catch", eLex.reserved);
			AddCommand("throw", eLex.reserved2);
			AddCommand("toString", eLex.reserved);
			AddCommand("valueOf", eLex.reserved);
			AddCommand("prototype", eLex.reserved);
			AddCommand("constructor", eLex.reserved);
			AddCommand("undefined", eLex.reserved);
			AddCommand("in", eLex.reserved3);
			AddCommand("do", eLex.reserved2);
			AddCommand("while", eLex.reserved2);
			AddCommand("arguments", eLex.reserved);
			AddCommand("length", eLex.reserved);
			AddCommand("versionSearch", eLex.reserved);
			AddCommand("identity", eLex.reserved);
			AddCommand("prop", eLex.reserved);
			AddCommand("versionSearchString", eLex.reserved);
			AddCommand("appVersion", eLex.reserved);
			AddCommand("userAgent", eLex.reserved);
			AddCommand("vendor", eLex.reserved);
			AddCommand("platform", eLex.reserved);
			AddCommand("substring", eLex.reserved);
			AddCommand("searchString", eLex.reserved);
			AddCommand("localStorage", eLex.reserved);
			AddCommand("hasOwnProperty", eLex.reserved);
			AddCommand("g_GameMakerHTML5Dir", eLex.reserved);
			AddCommand("facebookUI", eLex.reserved);
			AddCommand("g_fbAppHomeUrl", eLex.reserved);
			AddCommand("g_fbAppId", eLex.reserved);
			AddCommand("g_fbAppUrl", eLex.reserved);
			AddCommand("g_fbOAuthToken", eLex.reserved);
			AddCommand("navigator", eLex.reserved);
			AddCommand("browser", eLex.reserved);
			AddCommand("dataBrowser", eLex.reserved);
			AddCommand("version", eLex.reserved);
			AddCommand("searchVersion", eLex.reserved);
			AddCommand("dataOS", eLex.reserved);
			AddCommand("protocol", eLex.reserved);
			AddCommand("host", eLex.reserved);
			AddCommand("pathname", eLex.reserved);
			AddCommand("location", eLex.reserved);
			AddCommand("send", eLex.reserved);
			AddCommand("status", eLex.reserved);
			AddCommand("shiftKey", eLex.reserved);
			AddCommand("char", eLex.reserved);
			AddCommand("repeat", eLex.reserved);
			AddCommand("elementFromPoint", eLex.reserved);
			AddCommand("opera", eLex.reserved);
			AddCommand("write", eLex.reserved);
			AddCommand("open", eLex.reserved);
			AddCommand("XMLHttpRequest", eLex.reserved);
			AddCommand("ActiveXObject", eLex.reserved);
			AddCommand("setRequestHeader", eLex.reserved);
			AddCommand("responseText", eLex.reserved);
			AddCommand("XDomainRequest", eLex.reserved);
			AddCommand("status", eLex.reserved);
			AddCommand("ontimeout", eLex.reserved);
			AddCommand("onload", eLex.reserved);
			AddCommand("send", eLex.reserved);
			AddCommand("onreadystatechange", eLex.reserved);
			AddCommand("onerror", eLex.reserved);
			AddCommand("Math", eLex.reserved);
			AddCommand("Array", eLex.reserved);
			AddCommand("Date", eLex.reserved);
			AddCommand("window", eLex.reserved);
			AddCommand("document", eLex.reserved);
			AddCommand("Boolean", eLex.reserved);
			AddCommand("Number", eLex.reserved);
			AddCommand("domain", eLex.reserved);
			AddCommand("MAX_VALUE", eLex.reserved);
			AddCommand("MIN_VALUE", eLex.reserved);
			AddCommand("POSITIVE_INFINITY", eLex.reserved);
			AddCommand("NEGATIVE_INFINITY", eLex.reserved);
			AddCommand("toExponential", eLex.reserved);
			AddCommand("toPrecision", eLex.reserved);
			AddCommand("toFixed", eLex.reserved);
			AddCommand("charAt", eLex.reserved);
			AddCommand("charCodeAt", eLex.reserved);
			AddCommand("fromCharCode", eLex.reserved);
			AddCommand("indexOf", eLex.reserved);
			AddCommand("lastIndexOf", eLex.reserved);
			AddCommand("match", eLex.reserved);
			AddCommand("replace", eLex.reserved);
			AddCommand("search", eLex.reserved);
			AddCommand("split", eLex.reserved);
			AddCommand("substr", eLex.reserved);
			AddCommand("substring", eLex.reserved);
			AddCommand("toLowerCase", eLex.reserved);
			AddCommand("toUpperCase", eLex.reserved);
			AddCommand("anchor", eLex.reserved);
			AddCommand("big", eLex.reserved);
			AddCommand("blink", eLex.reserved);
			AddCommand("bold", eLex.reserved);
			AddCommand("fixed", eLex.reserved);
			AddCommand("fontcolor", eLex.reserved);
			AddCommand("fontsize", eLex.reserved);
			AddCommand("italics", eLex.reserved);
			AddCommand("link", eLex.reserved);
			AddCommand("small", eLex.reserved);
			AddCommand("strike", eLex.reserved);
			AddCommand("sub", eLex.reserved);
			AddCommand("sup", eLex.reserved);
			AddCommand("Infinity", eLex.reserved);
			AddCommand("NaN", eLex.reserved);
			AddCommand("decodeURI", eLex.reserved);
			AddCommand("decodeURIComponent", eLex.reserved);
			AddCommand("encodeURI", eLex.reserved);
			AddCommand("encodeURIComponent", eLex.reserved);
			AddCommand("escape", eLex.reserved);
			AddCommand("eval", eLex.reserved);
			AddCommand("sFinite", eLex.reserved);
			AddCommand("isNaN", eLex.reserved);
			AddCommand("prompt", eLex.reserved);
			AddCommand("parseFloat", eLex.reserved);
			AddCommand("parseInt", eLex.reserved);
			AddCommand("String", eLex.reserved);
			AddCommand("unescape", eLex.reserved);
			AddCommand("setInterval", eLex.reserved);
			AddCommand("JSON", eLex.reserved);
			AddCommand("stringify", eLex.reserved);
			AddCommand("global", eLex.reserved);
			AddCommand("ignoreCase", eLex.reserved);
			AddCommand("lastIndex", eLex.reserved);
			AddCommand("multiline", eLex.reserved);
			AddCommand("source", eLex.reserved);
			AddCommand("compile", eLex.reserved);
			AddCommand("exec", eLex.reserved);
			AddCommand("test", eLex.reserved);
			AddCommand("href ", eLex.reserved);
			AddCommand("button", eLex.reserved);
			AddCommand("pageX", eLex.reserved);
			AddCommand("pageY", eLex.reserved);
			AddCommand("splice", eLex.reserved);
			AddCommand("sort", eLex.reserved);
			AddCommand("concat", eLex.reserved);
			AddCommand("join", eLex.reserved);
			AddCommand("pop", eLex.reserved);
			AddCommand("push", eLex.reserved);
			AddCommand("reverse", eLex.reserved);
			AddCommand("shift", eLex.reserved);
			AddCommand("slice", eLex.reserved);
			AddCommand("nshift", eLex.reserved);
			AddCommand("random", eLex.reserved);
			AddCommand("round", eLex.reserved);
			AddCommand("LN2", eLex.reserved);
			AddCommand("LN10", eLex.reserved);
			AddCommand("LOG2E", eLex.reserved);
			AddCommand("PI", eLex.reserved);
			AddCommand("SQRT1_2", eLex.reserved);
			AddCommand("SQRT2", eLex.reserved);
			AddCommand("abs", eLex.reserved);
			AddCommand("acos", eLex.reserved);
			AddCommand("asin", eLex.reserved);
			AddCommand("atan", eLex.reserved);
			AddCommand("atan2", eLex.reserved);
			AddCommand("ceil", eLex.reserved);
			AddCommand("cos", eLex.reserved);
			AddCommand("exp", eLex.reserved);
			AddCommand("floor", eLex.reserved);
			AddCommand("log", eLex.reserved);
			AddCommand("max", eLex.reserved);
			AddCommand("min", eLex.reserved);
			AddCommand("pow", eLex.reserved);
			AddCommand("sin", eLex.reserved);
			AddCommand("sqrt", eLex.reserved);
			AddCommand("tan", eLex.reserved);
			AddCommand("getDate", eLex.reserved);
			AddCommand("getDay", eLex.reserved);
			AddCommand("getFullYear", eLex.reserved);
			AddCommand("getHours", eLex.reserved);
			AddCommand("getMilliseconds", eLex.reserved);
			AddCommand("getMinutes", eLex.reserved);
			AddCommand("getMonth", eLex.reserved);
			AddCommand("getSeconds", eLex.reserved);
			AddCommand("getTime", eLex.reserved);
			AddCommand("getTimezoneOffset", eLex.reserved);
			AddCommand("getUTCDate", eLex.reserved);
			AddCommand("getUTCDay", eLex.reserved);
			AddCommand("getUTCFullYear", eLex.reserved);
			AddCommand("getUTCHours", eLex.reserved);
			AddCommand("getUTCMilliseconds", eLex.reserved);
			AddCommand("getUTCMinutes", eLex.reserved);
			AddCommand("getUTCMonth", eLex.reserved);
			AddCommand("getUTCSeconds", eLex.reserved);
			AddCommand("parse", eLex.reserved);
			AddCommand("setDate", eLex.reserved);
			AddCommand("setFullYear", eLex.reserved);
			AddCommand("setHours", eLex.reserved);
			AddCommand("setMilliseconds", eLex.reserved);
			AddCommand("setMinutes", eLex.reserved);
			AddCommand("setMonth", eLex.reserved);
			AddCommand("setSeconds", eLex.reserved);
			AddCommand("setTime", eLex.reserved);
			AddCommand("setUTCDate", eLex.reserved);
			AddCommand("setUTCFullYear", eLex.reserved);
			AddCommand("setUTCHours", eLex.reserved);
			AddCommand("setUTCMilliseconds", eLex.reserved);
			AddCommand("setUTCMinutes", eLex.reserved);
			AddCommand("setUTCMonth", eLex.reserved);
			AddCommand("setUTCSeconds", eLex.reserved);
			AddCommand("toDateString", eLex.reserved);
			AddCommand("toLocaleDateString", eLex.reserved);
			AddCommand("toLocaleTimeString", eLex.reserved);
			AddCommand("toLocaleString", eLex.reserved);
			AddCommand("toTimeString", eLex.reserved);
			AddCommand("toUTCString", eLex.reserved);
			AddCommand("UTC", eLex.reserved);
			AddCommand("toDataURL", eLex.reserved);
			AddCommand("ontouchstart", eLex.reserved);
			AddCommand("ontouchmove", eLex.reserved);
			AddCommand("ontouchend", eLex.reserved);
			AddCommand("ontouchcancel", eLex.reserved);
			AddCommand("requestAnimationFrame", eLex.reserved);
			AddCommand("webkitRequestAnimationFrame", eLex.reserved);
			AddCommand("mozRequestAnimationFrame", eLex.reserved);
			AddCommand("oRequestAnimationFrame", eLex.reserved);
			AddCommand("msRequestAnimationFrame", eLex.reserved);
			AddCommand("setTimeout", eLex.reserved);
			AddCommand("onload", eLex.reserved);
			AddCommand("onerror", eLex.reserved);
			AddCommand("requestAnimFrame", eLex.reserved);
			AddCommand("innerWidth", eLex.reserved);
			AddCommand("innerHeight", eLex.reserved);
			AddCommand("console", eLex.reserved);
			AddCommand("which", eLex.reserved);
			AddCommand("Image", eLex.reserved);
			AddCommand("naturalWidth", eLex.reserved);
			AddCommand("naturalHeight", eLex.reserved);
			AddCommand("complete", eLex.reserved);
			AddCommand("crossOrigin", eLex.reserved);
			AddCommand("isMap", eLex.reserved);
			AddCommand("alt", eLex.reserved);
			AddCommand("src", eLex.reserved);
			AddCommand("img", eLex.reserved);
			AddCommand("ismap", eLex.reserved);
			AddCommand("usemap", eLex.reserved);
			AddCommand("title", eLex.reserved);
			AddCommand("figure", eLex.reserved);
			AddCommand("meta", eLex.reserved);
			AddCommand("name", eLex.reserved);
			AddCommand("generator", eLex.reserved);
			AddCommand("data", eLex.reserved);
			AddCommand("mageData", eLex.reserved);
			AddCommand("context", eLex.reserved);
			AddCommand("putImageData", eLex.reserved);
			AddCommand("srcElement", eLex.reserved);
			AddCommand("currentTarget", eLex.reserved);
			AddCommand("cursor", eLex.reserved);
			AddCommand("getElementById", eLex.reserved);
			AddCommand("appendChild", eLex.reserved);
			AddCommand("body", eLex.reserved);
			AddCommand("documentElement", eLex.reserved);
			AddCommand("clientWidth", eLex.reserved);
			AddCommand("clientHeight", eLex.reserved);
			AddCommand("getElementsByTagName", eLex.reserved);
			AddCommand("getAttribute", eLex.reserved);
			AddCommand("target", eLex.reserved);
			AddCommand("createEvent", eLex.reserved);
			AddCommand("initMouseEvent", eLex.reserved);
			AddCommand("dispatchEvent", eLex.reserved);
			AddCommand("event", eLex.reserved);
			AddCommand("preventDefault", eLex.reserved);
			AddCommand("type", eLex.reserved);
			AddCommand("changedTouches", eLex.reserved);
			AddCommand("screenX", eLex.reserved);
			AddCommand("screenY", eLex.reserved);
			AddCommand("clientX", eLex.reserved);
			AddCommand("clientY", eLex.reserved);
			AddCommand("addEventListener", eLex.reserved);
			AddCommand("removeEventListener", eLex.reserved);
			AddCommand("createElement", eLex.reserved);
			AddCommand("onmousemove", eLex.reserved);
			AddCommand("onmousedown", eLex.reserved);
			AddCommand("onmouseup", eLex.reserved);
			AddCommand("onkeydown", eLex.reserved);
			AddCommand("onkeyup", eLex.reserved);
			AddCommand("onfocusin", eLex.reserved);
			AddCommand("onfocusout", eLex.reserved);
			AddCommand("style", eLex.reserved);
			AddCommand("display", eLex.reserved);
			AddCommand("insertBefore", eLex.reserved);
			AddCommand("parent", eLex.reserved);
			AddCommand("parentNode", eLex.reserved);
			AddCommand("removeChild", eLex.reserved);
			AddCommand("frames", eLex.reserved);
			AddCommand("focus", eLex.reserved);
			AddCommand("oncontextmenu", eLex.reserved);
			AddCommand("now", eLex.reserved);
			AddCommand("visibility", eLex.reserved);
			AddCommand("textLength", eLex.reserved);
			AddCommand("setSelectionRange", eLex.reserved);
			AddCommand("alert", eLex.reserved);
			AddCommand("value", eLex.reserved);
			AddCommand("confirm", eLex.reserved);
			AddCommand("nextSibling", eLex.reserved);
			AddCommand("contentWindow", eLex.reserved);
			AddCommand("mozImageSmoothingEnabled", eLex.reserved);
			AddCommand("globalAlpha", eLex.reserved);
			AddCommand("strokeStyle", eLex.reserved);
			AddCommand("fillStyle", eLex.reserved);
			AddCommand("lineStyle", eLex.reserved);
			AddCommand("fillRect", eLex.reserved);
			AddCommand("strokeRect", eLex.reserved);
			AddCommand("getContext", eLex.reserved);
			AddCommand("width", eLex.reserved);
			AddCommand("height", eLex.reserved);
			AddCommand("offsetLeft", eLex.reserved);
			AddCommand("offsetTop", eLex.reserved);
			AddCommand("offsetParent", eLex.reserved);
			AddCommand("drawImage", eLex.reserved);
			AddCommand("left", eLex.reserved);
			AddCommand("position", eLex.reserved);
			AddCommand("beginPath", eLex.reserved);
			AddCommand("arc", eLex.reserved);
			AddCommand("fill", eLex.reserved);
			AddCommand("lineWidth", eLex.reserved);
			AddCommand("moveTo", eLex.reserved);
			AddCommand("lineTo", eLex.reserved);
			AddCommand("stroke", eLex.reserved);
			AddCommand("closePath", eLex.reserved);
			AddCommand("setTransform", eLex.reserved);
			AddCommand("save", eLex.reserved);
			AddCommand("restore", eLex.reserved);
			AddCommand("rect", eLex.reserved);
			AddCommand("transform", eLex.reserved);
			AddCommand("clip", eLex.reserved);
			AddCommand("top", eLex.reserved);
			AddCommand("hanging", eLex.reserved);
			AddCommand("middle", eLex.reserved);
			AddCommand("alphabetic", eLex.reserved);
			AddCommand("ideographic", eLex.reserved);
			AddCommand("bottom", eLex.reserved);
			AddCommand("fillText", eLex.reserved);
			AddCommand("strokeText", eLex.reserved);
			AddCommand("font", eLex.reserved);
			AddCommand("textAlign", eLex.reserved);
			AddCommand("textBaseline", eLex.reserved);
			AddCommand("measureText", eLex.reserved);
			AddCommand("HTMLVideoElement", eLex.reserved);
			AddCommand("HTMLCanvasElement", eLex.reserved);
			AddCommand("HTMLImageElement", eLex.reserved);
			AddCommand("video", eLex.reserved);
			AddCommand("readyState", eLex.reserved);
			AddCommand("onreadystatechange", eLex.reserved);
			AddCommand("CanvasPixelArray", eLex.reserved);
			AddCommand("shadowColor", eLex.reserved);
			AddCommand("shadowOffsetX", eLex.reserved);
			AddCommand("shadowOffsetY", eLex.reserved);
			AddCommand("shadowBlur", eLex.reserved);
			AddCommand("quadraticCurveTo", eLex.reserved);
			AddCommand("bezierCurveTo", eLex.reserved);
			AddCommand("arcTo", eLex.reserved);
			AddCommand("drawSystemFocusRing", eLex.reserved);
			AddCommand("drawCustomFocusRing", eLex.reserved);
			AddCommand("scrollPathIntoView", eLex.reserved);
			AddCommand("isPointInPath", eLex.reserved);
			AddCommand("lineCap", eLex.reserved);
			AddCommand("lineJoin", eLex.reserved);
			AddCommand("miterLimit", eLex.reserved);
			AddCommand("globalCompositeOperation", eLex.reserved);
			AddCommand("CanvasGradient", eLex.reserved);
			AddCommand("CanvasPattern", eLex.reserved);
			AddCommand("TextMetrics", eLex.reserved);
			AddCommand("addColorStop", eLex.reserved);
			AddCommand("createLinearGradient", eLex.reserved);
			AddCommand("createRadialGradient", eLex.reserved);
			AddCommand("createPattern", eLex.reserved);
			AddCommand("toBlob", eLex.reserved);
			AddCommand("setAttribute", eLex.reserved);
			AddCommand("scale", eLex.reserved);
			AddCommand("rotate", eLex.reserved);
			AddCommand("translate", eLex.reserved);
			AddCommand("clearRect", eLex.reserved);
			AddCommand("getImageData", eLex.reserved);
			AddCommand("createImageData", eLex.reserved);
			AddCommand("getter", eLex.reserved);
			AddCommand("setter", eLex.reserved);
			AddCommand("Audio", eLex.reserved);
			AddCommand("controls", eLex.reserved);
			AddCommand("src", eLex.reserved);
			AddCommand("load", eLex.reserved);
			AddCommand("autobuffer", eLex.reserved);
			AddCommand("loop", eLex.reserved);
			AddCommand("preload", eLex.reserved);
			AddCommand("play", eLex.reserved);
			AddCommand("pause", eLex.reserved);
			AddCommand("currentTime", eLex.reserved);
			AddCommand("volume", eLex.reserved);
			AddCommand("cloneNode", eLex.reserved);
			AddCommand("canplaythrough", eLex.reserved);
			AddCommand("error", eLex.reserved);
			AddCommand("onloaddata", eLex.reserved);
			AddCommand("canplay", eLex.reserved);
			AddCommand("canPlayType", eLex.reserved);
			AddCommand("viewportWidth", eLex.reserved);
			AddCommand("viewportHeight", eLex.reserved);
			AddCommand("createShader", eLex.reserved);
			AddCommand("getError", eLex.reserved);
			AddCommand("compileShader", eLex.reserved);
			AddCommand("shaderSource", eLex.reserved);
			AddCommand("getShaderParameter", eLex.reserved);
			AddCommand("attachShader", eLex.reserved);
			AddCommand("VERTEX_SHADER", eLex.reserved);
			AddCommand("FRAGMENT_SHADER", eLex.reserved);
			AddCommand("getShaderInfoLog", eLex.reserved);
			AddCommand("COMPILE_STATUS", eLex.reserved);
			AddCommand("createProgram", eLex.reserved);
			AddCommand("getProgramParameter", eLex.reserved);
			AddCommand("LINK_STATUS", eLex.reserved);
			AddCommand("useProgram", eLex.reserved);
			AddCommand("getUniformLocation", eLex.reserved);
			AddCommand("getAttribLocation", eLex.reserved);
			AddCommand("disable", eLex.reserved);
			AddCommand("CULL_FACE", eLex.reserved);
			AddCommand("createTexture", eLex.reserved);
			AddCommand("bindTexture", eLex.reserved);
			AddCommand("pixelStorei", eLex.reserved);
			AddCommand("TEXTURE_2D", eLex.reserved);
			AddCommand("UNPACK_WEBGL", eLex.reserved);
			AddCommand("RGBA", eLex.reserved);
			AddCommand("UNSIGNED_BYTE", eLex.reserved);
			AddCommand("TEXTURE_MAG_FILTER", eLex.reserved);
			AddCommand("TEXTURE_MIN_FILTER", eLex.reserved);
			AddCommand("NEAREST", eLex.reserved);
			AddCommand("LINEAR", eLex.reserved);
			AddCommand("TEXTURE_WRAP_S", eLex.reserved);
			AddCommand("TEXTURE_WRAP_T", eLex.reserved);
			AddCommand("CLAMP_TO_EDGE", eLex.reserved);
			AddCommand("COLOR_BUFFER_BIT", eLex.reserved);
			AddCommand("DEPTH_BUFFER_BIT", eLex.reserved);
			AddCommand("vertexAttribPointer", eLex.reserved);
			AddCommand("enableVertexAttribArray", eLex.reserved);
			AddCommand("bindBuffer", eLex.reserved);
			AddCommand("ARRAY_BUFFER", eLex.reserved);
			AddCommand("SHORT", eLex.reserved);
			AddCommand("BYTE", eLex.reserved);
			AddCommand("UNSIGNED_BYTE", eLex.reserved);
			AddCommand("FIXED", eLex.reserved);
			AddCommand("FLOAT", eLex.reserved);
			AddCommand("UNSIGNED_SHORT", eLex.reserved);
			AddCommand("uniform2f", eLex.reserved);
			AddCommand("uniform1i", eLex.reserved);
			AddCommand("activeTexture", eLex.reserved);
			AddCommand("clear", eLex.reserved);
			AddCommand("clearColor", eLex.reserved);
			AddCommand("uniformMatrix4fv", eLex.reserved);
			AddCommand("linkProgram", eLex.reserved);
			AddCommand("Int16Array", eLex.reserved);
			AddCommand("Int32Array", eLex.reserved);
			AddCommand("DYNAMIC_DRAW", eLex.reserved);
			AddCommand("Float32Array", eLex.reserved);
			AddCommand("texImage2D", eLex.reserved);
			AddCommand("createBuffer", eLex.reserved);
			AddCommand("texParameteri", eLex.reserved);
			AddCommand("bufferData", eLex.reserved);
			AddCommand("bufferSubData", eLex.reserved);
			AddCommand("enable", eLex.reserved);
			AddCommand("drawArrays", eLex.reserved);
			AddCommand("blendFunc", eLex.reserved);
			AddCommand("SRC_ALPHA", eLex.reserved);
			AddCommand("ONE_MINUS_SRC_ALPHA", eLex.reserved);
			AddCommand("BLEND", eLex.reserved);
			AddCommand("blendFuncSeparate", eLex.reserved);
			AddCommand("ONE", eLex.reserved);
			AddCommand("ZERO", eLex.reserved);
			AddCommand("TEXTURE0", eLex.reserved);
			AddCommand("TEXTURE1", eLex.reserved);
			AddCommand("viewport", eLex.reserved);
			AddCommand("bindFramebuffer", eLex.reserved);
			AddCommand("createFramebuffer", eLex.reserved);
			AddCommand("createRenderbuffer", eLex.reserved);
			AddCommand("bindRenderbuffer", eLex.reserved);
			AddCommand("renderbufferStorage", eLex.reserved);
			AddCommand("framebufferRenderbuffer", eLex.reserved);
			AddCommand("bindFramebuffer", eLex.reserved);
			AddCommand("framebufferTexture2D", eLex.reserved);
			AddCommand("deleteFramebuffer", eLex.reserved);
			AddCommand("deleteRenderbuffer", eLex.reserved);
			AddCommand("deleteTexture", eLex.reserved);
			AddCommand("readPixels", eLex.reserved);
			AddCommand("Uint8Array", eLex.reserved);
			AddCommand("FRAMEBUFFER", eLex.reserved);
			AddCommand("RENDERBUFFER", eLex.reserved);
			AddCommand("DEPTH_COMPONENT16", eLex.reserved);
			AddCommand("COLOR_ATTACHMENT0", eLex.reserved);
			AddCommand("colorMask", eLex.reserved);
			AddCommand("TRIANGLES", eLex.reserved);
			AddCommand("POINTS", eLex.reserved);
			AddCommand("LINE_STRIP", eLex.reserved);
			AddCommand("LINE_LOOP", eLex.reserved);
			AddCommand("TRIANGLE_STRIP", eLex.reserved);
			AddCommand("TRIANGLE_FAN", eLex.reserved);
			AddCommand("LINES", eLex.reserved);
			AddCommand("POINT", eLex.reserved);
			AddCommand("aa_1241_kz", eLex.reserved);
		}

		public eLex CheckToken(string _text)
		{
			eLex value;
			if (m_CommandLookup.TryGetValue(_text, out value))
			{
				return value;
			}
			return eLex.None;
		}

		public void Push()
		{
			m_Tail--;
			if (m_Tail < 0)
			{
				m_Tail = 99;
			}
		}

		public void EnqueueLex(eLex _token, string _text, double _value)
		{
			m_Head++;
			if (m_Head == 100)
			{
				m_Head = 0;
			}
			m_Tail++;
			if (m_Tail == 100)
			{
				m_Tail = 0;
			}
			m_LexToken[m_Head] = _token;
			m_LexText[m_Head] = _text;
			m_LexValue[m_Head] = _value;
		}

		public char NextChar()
		{
			if (m_Index >= m_Text.Length)
			{
				return ' ';
			}
			return m_Text[m_Index++];
		}

		private eLex ReadID()
		{
			int num = m_Index - 1;
			while (m_Index < m_Text.Length)
			{
				char c = NextChar();
				if (m_IDLookup[c] != eLex.ID)
				{
					m_Index--;
					break;
				}
			}
			int index = m_Index;
			string text = m_Text.Substring(num, index - num);
			eLex value;
			if (m_CommandLookup.TryGetValue(text, out value))
			{
				EnqueueLex(value, text, 0.0);
				return value;
			}
			EnqueueLex(eLex.ID, text, 0.0);
			return eLex.ID;
		}

		private eLex ReadHEX()
		{
			int index = m_Index;
			m_Index += 2;
			long num = 0L;
			while (m_Index < m_Text.Length)
			{
				char c = NextChar();
				int num2 = m_HexLookup[c];
				if (num2 == -1)
				{
					m_Index--;
					break;
				}
				num = (num << 4) + num2;
			}
			int index2 = m_Index;
			string text = m_Text.Substring(index, index2 - index);
			EnqueueLex(eLex.Decimal, text, num);
			return eLex.Decimal;
		}

		private eLex ReadBIN()
		{
			int num = m_Index - 1;
			long num2 = 0L;
			while (m_Index < m_Text.Length)
			{
				char c = NextChar();
				int num3 = m_BinLookup[c];
				if (num3 == -1)
				{
					m_Index--;
					break;
				}
				num2 = (num2 << 1) + num3;
			}
			int index = m_Index;
			string text = m_Text.Substring(num, index - num);
			EnqueueLex(eLex.Decimal, text, num2);
			return eLex.Decimal;
		}

		private eLex ReadString1()
		{
			if (!m_pObfuscate.EncodeStrings)
			{
				int num = m_Index - 1;
				long num2 = 0L;
				while (m_Index < m_Text.Length)
				{
					switch (NextChar())
					{
					case '\\':
					{
						char c = NextChar();
						continue;
					}
					default:
						continue;
					case '"':
						break;
					}
					break;
				}
				int index = m_Index;
				string text = m_Text.Substring(num, index - num);
				EnqueueLex(eLex.String, text, num2);
				return eLex.String;
			}
			long num3 = 0L;
			char c2;
			string text2;
			for (text2 = "\""; m_Index < m_Text.Length; text2 += c2)
			{
				c2 = NextChar();
				switch (c2)
				{
				case '\\':
					c2 = NextChar();
					continue;
				default:
					continue;
				case '"':
					break;
				}
				break;
			}
			text2 += "\"";
			EnqueueLex(eLex.String, text2, num3);
			return eLex.String;
		}

		private eLex ReadString2()
		{
			if (!m_pObfuscate.EncodeStrings)
			{
				int num = m_Index - 1;
				long num2 = 0L;
				while (m_Index < m_Text.Length)
				{
					switch (NextChar())
					{
					case '\\':
					{
						char c = NextChar();
						continue;
					}
					default:
						continue;
					case '\'':
						break;
					}
					break;
				}
				int index = m_Index;
				string text = m_Text.Substring(num, index - num);
				EnqueueLex(eLex.String, text, num2);
				return eLex.String;
			}
			long num3 = 0L;
			char c2;
			string text2;
			for (text2 = "\""; m_Index < m_Text.Length; text2 += c2)
			{
				c2 = NextChar();
				switch (c2)
				{
				case '\\':
					c2 = NextChar();
					continue;
				default:
					continue;
				case '\'':
					break;
				}
				break;
			}
			text2 += "\"";
			EnqueueLex(eLex.String, text2, num3);
			return eLex.String;
		}

		private eLex ReadDec()
		{
			m_Index--;
			if (m_Index + 3 < m_Text.Length && m_Text[m_Index] == '0' && (m_Text[m_Index + 1] == 'x' || m_Text[m_Index + 2] == 'X'))
			{
				return ReadHEX();
			}
			if (m_Index + 2 < m_Text.Length && m_Text[m_Index] == '1' && (m_Text[m_Index + 1] == 'e' || m_Text[m_Index + 1] == 'E') && (m_Text[m_Index + 2] == '+' || m_Text[m_Index + 2] == '-'))
			{
				string text = string.Format("{0}{1}{2}", m_Text[m_Index], m_Text[m_Index + 1], m_Text[m_Index + 2]);
				m_Index += 3;
				while (m_Index < m_Text.Length)
				{
					char c = NextChar();
					int num = m_DecLookup[c];
					if (num == -1)
					{
						m_Index--;
						break;
					}
					text += c;
				}
				text = text.ToLower();
				double value = Convert.ToDouble(text);
				EnqueueLex(eLex.Decimal, text, value);
				return eLex.Decimal;
			}
			int index = m_Index;
			long num2 = 0L;
			while (m_Index < m_Text.Length)
			{
				char c2 = NextChar();
				int num3 = m_DecLookup[c2];
				if (num3 == -1)
				{
					m_Index--;
					break;
				}
				num2 = num2 * 10 + num3;
			}
			int index2 = m_Index;
			string text2 = m_Text.Substring(index, index2 - index);
			EnqueueLex(eLex.Decimal, text2, num2);
			return eLex.Decimal;
		}

		private eLex CheckForComments()
		{
			m_Index--;
			if (m_Index < m_Text.Length)
			{
				char c = NextChar();
				char c2 = NextChar();
				eLex eLex = m_TokenLookup[c];
				eLex eLex2 = m_TokenLookup[c2];
				if (eLex == eLex.Divide && eLex2 == eLex.Divide)
				{
					while (m_Index < m_Text.Length)
					{
						c = NextChar();
						eLex = eLex.ID;
						if (c >= '\0' && c <= 'ÿ')
						{
							eLex = m_TokenLookup[c];
						}
						if (eLex == eLex.EOF || eLex == eLex.NewLine)
						{
							c = NextChar();
							if (eLex != eLex.EOF && eLex != eLex.NewLine)
							{
								m_Index--;
							}
							break;
						}
					}
					return eLex.Comment;
				}
				if (eLex == eLex.Divide && eLex2 == eLex.Star)
				{
					int num = 1;
					while (m_Index < m_Text.Length)
					{
						c = NextChar();
						c2 = NextChar();
						eLex = eLex.ID;
						eLex2 = eLex.ID;
						if (c >= '\0' && c <= 'ÿ')
						{
							eLex = m_TokenLookup[c];
						}
						if (c2 >= '\0' && c2 <= 'ÿ')
						{
							eLex2 = m_TokenLookup[c2];
						}
						if (eLex == eLex.Divide && eLex2 == eLex.Star)
						{
							num++;
						}
						if (eLex == eLex.Star && eLex2 == eLex.Divide)
						{
							num--;
							if (num == 0)
							{
								break;
							}
						}
						m_Index--;
					}
					return eLex.Comment;
				}
				m_Index--;
				EnqueueLex(eLex.Divide, "/", 0.0);
				return eLex.Divide;
			}
			return GMAssetCompiler.eLex.Divide;
		}

		public eLex MultiReadToken(char _c, eLex _token)
		{
			if (m_Index < m_Text.Length)
			{
				char c = NextChar();
				eLex eLex = m_SymbolLookup2[c];
				if (eLex != 0)
				{
					EnqueueLex(eLex, new string(new char[2]
					{
						_c,
						c
					}), 0.0);
					return eLex;
				}
				m_Index--;
			}
			EnqueueLex(_token, new string(new char[1]
			{
				_c
			}), 0.0);
			return _token;
		}

		public eLex yylex()
		{
			if (m_Tail != m_Head)
			{
				m_Tail++;
				if (m_Tail == 100)
				{
					m_Tail = 0;
				}
				return m_LexToken[m_Tail];
			}
			while (m_Index < m_Text.Length)
			{
				char c = NextChar();
				eLex eLex = eLex.ID;
				if (c >= '\0' && c <= 'ÿ')
				{
					eLex = m_TokenLookup[c];
				}
				switch (eLex)
				{
				case eLex.WhiteSpace:
					continue;
				case eLex.Divide:
					return CheckForComments();
				case eLex.None:
					return ReadID();
				case eLex.Quotes:
					return ReadString1();
				case eLex.SingleQuotes:
					return ReadString2();
				case eLex.Decimal:
					return ReadDec();
				}
				if (m_SymbolLookup1[c] != 0)
				{
					return MultiReadToken(c, eLex);
				}
				EnqueueLex(eLex, new string(new char[1]
				{
					c
				}), 0.0);
				return eLex;
			}
			EnqueueLex(GMAssetCompiler.eLex.EOF, null, 0.0);
			return GMAssetCompiler.eLex.EOF;
		}
	}
}
