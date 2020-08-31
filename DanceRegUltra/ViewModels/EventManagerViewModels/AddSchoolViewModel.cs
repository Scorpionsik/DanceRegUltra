using CoreWPF.MVVM;
using CoreWPF.Utilites;
using DanceRegUltra.Models.Categories;
using DanceRegUltra.Static;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanceRegUltra.ViewModels.EventManagerViewModels
{
    public class AddSchoolViewModel : ViewModel
    {
        private string schoolName;
        public string SchoolName
        {
            get => this.schoolName;
            set
            {
                this.schoolName = value;
                this.OnPropertyChanged("SchoolName");
            }
        }

        public AddSchoolViewModel() : base()
        {
            this.schoolName = "";
        }

        private async void SaveSchool()
        {
            await DanceRegDatabase.ExecuteNonQueryAsync("insert into schools ('Name') values ('" + this.SchoolName + "')");
            DbResult res = await DanceRegDatabase.ExecuteAndGetQueryAsync("select * from schools order by Id_school");
            IdTitle tmp_school = new IdTitle(res["Id_school", res.RowsCount - 1].ToInt32(), this.SchoolName);

            DanceRegCollections.Schools.Value.Add(tmp_school);

            base.Command_save?.Execute();
        }

        public override RelayCommand Command_save
        {
            get => new RelayCommand(obj =>
            {
                this.SaveSchool();
            },
                (obj) => this.SchoolName != null && this.SchoolName.Length > 0);
        }
    }
}
