﻿using AssetRipper.IO;

namespace AssetRipper.Reading.Classes
{
	public abstract class Component : EditorExtension
    {
        public PPtr<GameObject> m_GameObject;

        protected Component(ObjectReader reader) : base(reader)
        {
            m_GameObject = new PPtr<GameObject>(reader);
        }
    }
}