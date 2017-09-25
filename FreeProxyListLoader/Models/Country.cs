namespace FreeProxyListLoader.Models
{
    class Country
    {
        public string Code { set; get; }
        public string Name { set; get; }

        public Country(string code, string name)
        {
            Code = code;
            Name = name;
        }
    }
}
