using System;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

namespace FairyGame
{
	public class ModalWaiting : GComponent
	{
		GObject _obj;
		float _visCounter;

		public override void ConstructFromXML(FairyGUI.Utils.XML xml)
		{
			base.ConstructFromXML(xml);

			_obj = GetChild("n0");

			this.onAddedToStage.Add(OnAddedToStage);
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();

			_visCounter += Time.deltaTime;
			if (_visCounter >= 1.5f && !_obj.visible)
				_obj.visible = true;

			_obj.rotation += 8;
		}

		void OnAddedToStage()
		{
			if (parent is Window)
			{
				_obj.visible = false;
				_visCounter = 0;
			}
			else
				_obj.visible = true;
		}
	}
}
