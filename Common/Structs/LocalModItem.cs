namespace ModManager.Common.Structs
{
	public class LocalModItem
    {
		private int? curseforgemodid;
		/// <summary>
		/// Curseforge的Mod ID
		/// </summary>
		public int? CurseforgeModID
		{
			get { return curseforgemodid; }
			set { curseforgemodid = value; }
		}

		private string? modrinthmodid;
		/// <summary>
		/// Modrinth的Mod ID
		/// </summary>
		public string? ModrinthModID
		{
			get { return modrinthmodid; }
			set { modrinthmodid = value; }
		}

		private string? murmurhash2value;
		/// <summary>
		/// MurmurHash2的值
		/// </summary>
		public string? MurmurHash2Value
		{
			get { return murmurhash2value; }
			set { murmurhash2value = value; }
		}

		private string? sha1value;
		/// <summary>
		/// SHA1的值
		/// </summary>
		public string? Sha1Value
		{
			get { return sha1value; }
			set { sha1value = value; }
		}

		private string? filepath;
		/// <summary>
		/// 本地文件的路径
		/// </summary>
		public string? FilePath
		{
			get { return filepath; }
			set { filepath = value; }
		}



	}
}
