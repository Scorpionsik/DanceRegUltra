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
    public class LeagueMenuElementViewModel : NavigationModel, ICategoriesMenu
    {
        public LeagueMenuElementViewModel(NavigationManager nav) : base(CategoryType.League.ToString(), nav)
        {
            
        }

        private ListExt<CategoryString> categorys;
        public ListExt<CategoryString> Categorys
        {
            get
            {
                ListExt<CategoryString> result = new ListExt<CategoryString>();
                foreach(CategoryString value in DanceRegCollections.Leagues.Value)
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

        private async void AddLeagueMethod(string league_name)
        {
            bool isBeginAdd = await Task.Run<bool>(() =>
            {
                foreach (CategoryString league in DanceRegCollections.Leagues.Value)
                {
                    if (league.Name == league_name)
                    {
                        if(league.IsHide) league.IsHide = false;
                        return false;
                    }
                }
                return true;
            });

            if (isBeginAdd)
            {
                await DanceRegDatabase.ExecuteNonQueryAsync("insert into leagues ('Name') values ('"+ league_name +"')");
                DbResult res = await DanceRegDatabase.ExecuteAndGetQueryAsync("select Id_league, Name from leagues order by Id_league");
                DbRow row = res.GetRow(res.RowsCount - 1);
                CategoryString add_league = new CategoryString(row.GetInt32("Id_league"), CategoryType.League, row["Name"].ToString());
                DanceRegCollections.LoadLeague(add_league);
            }
            this.OnPropertyChanged("Categorys");
        }

        public RelayCommand<string> Command_add
        {
            get => new RelayCommand<string>(name =>
            {
                this.AddLeagueMethod(name);
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
                int oldIndex = DanceRegCollections.Leagues.Value.IndexOf(category);

                int insert_index = dropInfo.InsertIndex;
                if (insert_index > oldIndex) insert_index -= 1;
                CategoryString new_category = this.categorys[insert_index];
                int newIndex = DanceRegCollections.Leagues.Value.IndexOf(new_category);
                
                

                DanceRegCollections.Leagues.Value.Move(oldIndex, newIndex);
                this.OnPropertyChanged("Categorys");

                await Task.Run(() =>
                {
                    int[] minmax = new int[2] { Math.Min(oldIndex, newIndex), Math.Max(oldIndex, newIndex) };

                    for (int i = minmax[0]; i <= minmax[1]; i++)
                    {
                        DanceRegCollections.Leagues.Value[i].Position = i + 1;
                    }
                });
                
            }
        }

        public override void OnNavigatingFrom() { }

        public override void OnNavigatingTo(object arg) { }
    }
}
