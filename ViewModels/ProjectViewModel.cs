using ModManager.Common.Structs;
using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace ModManager.ViewModels
{
	public class ProjectViewModel: BindableBase
    {
		ProjectViewModel()
		{
			createtestdata();

        }
		private ObservableCollection<ProjectItem> projects;
		public ObservableCollection<ProjectItem> Projects
        {
			get { return projects; }
			set { projects = value; RaisePropertyChanged(); }
		}

		void createtestdata()
		{
			Projects = new()
			{
				new ProjectItem()
				{
					Name = "test",
					FolderPath = "testpath",
					MCVersion = "1.19.2",
					LoaderType = "fabric",
					ModItems = null
				},
                new ProjectItem()
                {
                    Name = "test2",
                    FolderPath = "testpath",
                    MCVersion = "1.19.2",
                    LoaderType = "fabric",
                    ModItems = null
                }
            };
		}
	}
}
