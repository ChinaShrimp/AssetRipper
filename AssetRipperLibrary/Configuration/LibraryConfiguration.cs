﻿using AssetRipper.Core.Configuration;

namespace AssetRipper.Library.Configuration
{
	public class LibraryConfiguration : CoreConfiguration
	{
		/// <summary>
		/// The file format that audio clips get exported in. Recommended: Ogg
		/// </summary>
		public AudioExportFormat AudioExportFormat { get; set; } 
		/// <summary>
		/// The file format that images (like textures) get exported in.
		/// </summary>
		public ImageExportFormat ImageExportFormat { get; set; }
		/// <summary>
		/// The format that meshes get exported in. Currently does nothing.
		/// </summary>
		public MeshExportFormat MeshExportFormat { get; set; }
		/// <summary>
		/// Dummy shaders compile in the editor. <br/>
		/// Disassembled shaders provide insight into their precompiled state, but don't compile in the editor.
		/// </summary>
		public ShaderExportMode ShaderExportMode { get; set; } 
		/// <summary>
		/// Should sprites be exported as a texture? Recommended: Native
		/// </summary>
		public SpriteExportMode SpriteExportMode { get; set; } 

		public override void ResetToDefaultValues()
		{
			base.ResetToDefaultValues();
			AudioExportFormat = AudioExportFormat.Ogg;
			ImageExportFormat = ImageExportFormat.Png;
			MeshExportFormat = MeshExportFormat.Obj;
			ShaderExportMode = ShaderExportMode.Dummy;
			SpriteExportMode = SpriteExportMode.Native;
		}
	}
}
