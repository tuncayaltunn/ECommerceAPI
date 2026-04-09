using System;
namespace ETicaretAPI.Infrastructure.Operations
{
	public static class NameOperation
	{
		public static string CharacterRegulatory(string name)
		{
			name.Replace("ç", "c")
				.Replace("ğ", "g")
				.Replace("ı", "i")
				.Replace("ö", "o")
				.Replace("ş", "s")
				.Replace("ü", "u");
			return name;
		}
	}
}

