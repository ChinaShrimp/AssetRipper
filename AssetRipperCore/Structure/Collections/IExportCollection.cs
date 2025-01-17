﻿using AssetRipper.Core.Project;
using AssetRipper.Core.Classes.Meta;
using AssetRipper.Core.Classes.Object;
using AssetRipper.Core.Parser.Files.SerializedFiles;
using AssetRipper.Core.IO.Asset;
using System.Collections.Generic;

namespace AssetRipper.Core.Structure.Collections
{
	public interface IExportCollection
	{
		bool Export(ProjectAssetContainer container, string dirPath);
		bool IsContains(Object asset);
		long GetExportID(Object asset);
		MetaPtr CreateExportPointer(Object asset, bool isLocal);

		ISerializedFile File { get; }
		TransferInstructionFlags Flags { get; }
		IEnumerable<Object> Assets { get; }
		string Name { get; }
	}
}
