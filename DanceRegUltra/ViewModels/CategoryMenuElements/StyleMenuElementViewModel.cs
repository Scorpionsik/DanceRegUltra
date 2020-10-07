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
    public class StyleMenuElementViewModel : NavigationModel, ICategoriesMenu
    {
        public StyleMenuElementViewModel(NavigationManager nav) : base(CategoryType.Style.ToString(), nav)
        {

        }

        private ListExt<CategoryString> categorys;
        public ListExt<CategoryString> Categorys
        {
            get
            {
                ListExt<CategoryString> result = new ListExt<CategoryString>();
                foreach (CategoryString value in DanceRegCollections.Styles.Value)
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

        private async void AddStyleMethod(string style_name)
        {
            bool isBeginAdd = await Task.Run<bool>(() =>
            {
                foreach (CategoryString style in DanceRegCollections.Styles.Value)
                {
                    if (style.Name == style_name)
                    {
                        if (style.IsHide) style.IsHide = false;
                        return false;
                    }
                }
                return true;
            });

            if (isBeginAdd)
            {
                await DanceRegDatabase.ExecuteNonQueryAsync("insert into styles ('Name') values ('" + style_name + "')");
                DbResult res = await DanceRegDatabase.ExecuteAndGetQueryAsync("select Id_style, Name from styles order by Id_style");
                DbRow row = res.GetRow(res.RowsCount - 1);
                CategoryString add_style = new CategoryString(row.GetInt32("Id_style"), CategoryType.Style, row["Name"].ToString());
                DanceRegCollections.LoadStyle(add_style);
            }
            this.OnPropertyChanged("Categorys");
        }

        public RelayCommand<string> Command_add
        {
            get => new RelayCommand<string>(name =>
            {
                this.AddStyleMethod(name);
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
                int oldIndex = DanceRegCollections.Styles.Value.IndexOf(category);

                int insert_index = dropInfo.InsertIndex;
                if (insert_index > oldIndex) insert_index -= 1;
                CategoryString new_category = this.categorys[insert_index];
                int newIndex = DanceRegCollections.Styles.Value.IndexOf(new_category);



                DanceRegCollections.Styles.Value.Move(oldIndex, newIndex);
                this.OnPropertyChanged("Categorys");

                await Task.Run(() =>
                {
                    int[] minmax = new int[2] { Math.Min(oldIndex, newIndex), Math.Max(oldIndex, newIndex) };

                    for (int i = minmax[0]; i <= minmax[1]; i++)
                    {
                        DanceRegCollections.Styles.Value[i].Position = i + 1;
                    }
                });

            }
        }

        public override void OnNavigatingFrom() { }

        public override void OnNavigatingTo(object arg) { }
    }
}
