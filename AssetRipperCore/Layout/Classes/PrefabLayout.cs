﻿using AssetRipper.Core.Converters.Game;
using AssetRipper.Core.Classes;

namespace AssetRipper.Core.Layout.Classes
{
	/// <summary>
	/// 2018.3 - first introduction
	/// </summary>
	public sealed class PrefabLayout
	{
		public PrefabLayout(LayoutInfo info) { }

		public static void GenerateTypeTree(TypeTreeContext context)
		{
			PrefabLayout layout = context.Layout.Prefab;
			context.AddNode(layout.Name, TypeTreeUtils.BaseName);
			context.BeginChildren();
			ObjectLayout.GenerateTypeTree(context);
			context.AddPPtr(context.Layout.GameObject.Name, layout.RootGameObjectName);
			context.EndChildren();
		}

		public bool HasRootGameObject => true;

		public string Name => nameof(Prefab);
		public string RootGameObjectName => "m_RootGameObject";
	}
}
