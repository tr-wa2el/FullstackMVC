namespace FullstackMVC.ViewModels
{
    public class DeptWithEctraInfoVM
    {
        public string Name { get; set; }

        public string Message { get; set; }

        public string Manager { get; set; }

        public string Color { get; set; }

        public int Count { get; set; }

        public List<string> empsNames { get; set; } = new List<string>();

        public List<string> Branches { get; set; }
    }
}
