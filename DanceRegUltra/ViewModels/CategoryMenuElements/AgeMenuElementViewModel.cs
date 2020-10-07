using CoreWPF.Utilites;
using CoreWPF.Utilites.Navigation;
using DanceRegUltra.Enums;
using DanceRegUltra.Interfaces;
using DanceRegUltra.Models.Categories;
using DanceRegUltra.Static;
using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DanceRegUltra.ViewModels.CategoryMenuElements
{
    public class AgeMenuElementViewModel : NavigationModel, ICategoriesMenu
    {
        public AgeMenuElementViewModel(NavigationManager nav) : base(CategoryType.Age.ToString(), nav)
        {

        }

        private ListExt<CategoryString> categorys;
        public ListExt<CategoryString> Categorys
        {
            get
            {
                ListExt<CategoryString> result = new ListExt<CategoryString>();
                foreach (CategoryString value in DanceRegCollections.Ages.Value)
                {
                    if (!value.IsHide) result.Add(value);
                }
                this.categorys = new ListExt<CategoryString>(result);
                return result;
            }
        }

        private CategoryString select_category;

        public CategoryString Select_category
        {
            get => this.select_category;
            set
            {
                this.select_category = value;
                this.OnPropertyChanged("Select_category");
            }
        }

        private async void AddAgeMethod(string age_name)
        {
            bool isBeginAdd = await Task.Run<bool>(() =>
            {
                foreach (CategoryString age in DanceRegCollections.Ages.Value)
                {
                    if (age.Name == age_name)
                    {
                        age.IsHide = false;
                        return false;
                    }
                }
                return true;
            });

            if (isBeginAdd)
            {
                await DanceRegDatabase.ExecuteNonQueryAsync("insert into ages ('Name') values ('" + age_name + "')");
                DbResult res = await DanceRegDatabase.ExecuteAndGetQueryAsync("select Id_age, Name from ages order by Id_age");
                DbRow row = res.GetRow(res.RowsCount - 1);
                CategoryString add_age = new CategoryString(row.GetInt32("Id_age"), CategoryType.Age, row["Name"].ToString());
                DanceRegCollections.LoadAge(add_age);
            }
            this.OnPropertyChanged("Categorys");
        }

        public RelayCommand<string> Command_add
        {
            get => new RelayCommand<string>(name =>
            {
                this.AddAgeMethod(name);
            },
                (name) => name != null && name.Length > 0);
        }

        public RelayCommand Command_remove
        {
            get => new RelayCommand(obj =>
            {
                this.Select_category.IsHide = true;
                this.OnPropertyChanged("Categorys");
            },
                (obj) => this.Select_category != null);
        }

        public void DragOver(IDropInfo dropInfo)
        {
            dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
            dropInfo.Effects = DragDropEffects.Move;
        }

        public async void Drop(IDropInfo dropInfo)
        {
            if (dropInfo.Data is CategoryString category)
            {
                int oldIndex = DanceRegCollections.Ages.Value.IndexOf(category);

                int insert_index = dropInfo.InsertIndex;
                if (insert_index > oldIndex) insert_index -= 1;
                CategoryString new_category = this.categorys[insert_index];
                int newIndex = DanceRegCollections.Ages.Value.IndexOf(new_category);



                DanceRegCollections.Ages.Value.Move(oldIndex, newIndex);
                this.OnPropertyChanged("Categorys");

                await Task.Run(() =>
                {
                    int[] minmax = new int[2] { Math.Min(oldIndex, newIndex), Math.Max(oldIndex, newIndex) };

                    for (int i = minmax[0]; i <= minmax[1]; i++)
                    {
                        DanceRegCollections.Ages.Value[i].Position = i + 1;
                    }
                });

            }
        }

        public override void OnNavigatingFrom() { }

        public override void OnNavigatingTo(object arg) { }
    }
}
