﻿using AssetRipper.Core.IO;

namespace AssetRipper.Core.Reading.Classes
{
	public class ValueDelta
	{
		public float m_Start;
		public float m_Stop;

		public ValueDelta(ObjectReader reader)
		{
			m_Start = reader.ReadSingle();
			m_Stop = reader.ReadSingle();
		}
	}
}
