using System;
using System.Collections.Generic;
using FairyGUI.Utils;
//using LuaInterface;
using XLua;

namespace FairyGUI
{
	/// <summary>
	/// 
	/// </summary>
	public sealed class LuaUIHelper
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="url"></param>
		/// <param name="luaClass"></param>
		public static void SetExtension(string url, System.Type baseType, LuaFunction extendFunction)
		{ 
            UIObjectFactory.SetPackageItemExtension(url, () => {
				GComponent gcom = (GComponent)Activator.CreateInstance(baseType);
				gcom.data = extendFunction;
				return gcom;
			});
		}

		[LuaCallCSharp]
		public static LuaTable ConnectLua(GComponent gcom)
		{
			LuaTable _peerTable = null;
			LuaFunction extendFunction = gcom.data as LuaFunction;
			if (extendFunction!=null)
			{
				gcom.data = null;
                var result = extendFunction.Call(gcom);
                _peerTable = result[0] as LuaTable;
                //LuaManager.Instance.luaenv.Global.Get<LuaTable>
			}

			return _peerTable;
		}
	}

	public class GLuaComponent : GComponent
	{
		LuaTable _peerTable;

		//[LuaCallCSharp]
		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			_peerTable = LuaUIHelper.ConnectLua(this);
		}

		public override void Dispose()
		{
			base.Dispose();

			if (_peerTable != null)
				_peerTable.Dispose();
		}
	}

	public class GLuaLabel : GLabel
	{
		LuaTable _peerTable;

		//[LuaCallCSharp]
		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			_peerTable = LuaUIHelper.ConnectLua(this);
		}

		public override void Dispose()
		{
			base.Dispose();

			if (_peerTable != null)
				_peerTable.Dispose();
		}
	}

	public class GLuaButton : GButton
	{
		LuaTable _peerTable;

		////[NoToLuaAttribute]
		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			_peerTable = LuaUIHelper.ConnectLua(this);
		}

		public override void Dispose()
		{
			base.Dispose();

			if (_peerTable != null)
				_peerTable.Dispose();
		}
	}

	public class GLuaProgressBar : GProgressBar
	{
		LuaTable _peerTable;

		//[NoToLuaAttribute]
		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			_peerTable = LuaUIHelper.ConnectLua(this);
		}

		public override void Dispose()
		{
			base.Dispose();

			if (_peerTable != null)
				_peerTable.Dispose();
		}
	}

	public class GLuaSlider : GSlider
	{
		LuaTable _peerTable;

		//[NoToLuaAttribute]
		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			_peerTable = LuaUIHelper.ConnectLua(this);
		}

		public override void Dispose()
		{
			base.Dispose();

			if (_peerTable != null)
				_peerTable.Dispose();
		}
	}

	public class GLuaComboBox : GComboBox
	{
		LuaTable _peerTable;

		//[NoToLuaAttribute]
		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			_peerTable = LuaUIHelper.ConnectLua(this);
		}

		public override void Dispose()
		{
			base.Dispose();

			if (_peerTable != null)
				_peerTable.Dispose();
		}
	}

    [LuaCallCSharp]
    public class LuaWindow : Window
	{
		LuaTable _peerTable;
		//Action _OnInit;
        LuaFunction _OnInit;

        LuaFunction _DoHideAnimation;
        LuaFunction _DoShowAnimation;
        LuaFunction _OnShown;
        LuaFunction _OnHide;

		public void ConnectLua(LuaTable peerTable)
		{
            _peerTable = peerTable;
            _OnInit = _peerTable.Get<LuaFunction>("OnInit");
            _DoHideAnimation = _peerTable.Get<LuaFunction>("DoHideAnimation"); 
            _DoShowAnimation = _peerTable.Get<LuaFunction>("DoShowAnimation"); 
            _OnShown = _peerTable.Get<LuaFunction>("OnShown");
            _OnHide = _peerTable.Get<LuaFunction>("OnHide"); 
        }

		public override void Dispose()
		{
			base.Dispose();

			if (_peerTable != null)
				_peerTable.Dispose();
            if (_OnInit != null)
                _OnInit = null;  //.Dispose();
			if (_DoHideAnimation != null)
				_DoHideAnimation = null;
            if (_DoShowAnimation != null)
				_DoShowAnimation = null;  //.Dispose();
			if (_OnShown != null)
				_OnShown = null;  //.Dispose();
			if (_OnHide != null)
				_OnHide = null;  //.Dispose();
		}

		protected override void OnInit()
		{
            if (_OnInit != null)
                _OnInit.Call(_peerTable);
		}

		protected override void DoHideAnimation()
		{
			if (_DoHideAnimation != null)
			{
				_DoHideAnimation.Call(_peerTable);
			}
			else
				base.DoHideAnimation();
		}

		protected override void DoShowAnimation()
		{
			if (_DoShowAnimation != null)
			{
				_DoShowAnimation.Call(_peerTable);
			}
			else
				base.DoShowAnimation();
		}

		protected override void OnShown()
		{
			base.OnShown();

			if (_OnShown != null)
			{
				_OnShown.Call(_peerTable);
			}
		}

		protected override void OnHide()
		{
			base.OnHide();

			if (_OnHide != null)
			{
				_OnHide.Call(_peerTable);
			}
		}
	}
}