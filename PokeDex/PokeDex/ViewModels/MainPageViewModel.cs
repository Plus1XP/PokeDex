﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using PokeDex.Models;
using PokeDex.Views;

using Xamarin.Forms;

namespace PokeDex.ViewModels
{
    class MainPageViewModel : INotifyPropertyChanged
    {
        private PokedexManagerModel pkmManager;

        private DataManager dataManager;

        private DetailsPage detailsPageView;
        private DetailsPageViewModel detailsPageViewModel;

        private ObservableCollection<PokedexModel> searchSuggestionsCollection;

        private int pkmToFind = 151;

        private bool isSearchResultsListVisible = false;
        private string logoPath = $"Images/Original/Logo.png";

        public MainPageViewModel()
        {
            pkmManager = new PokedexManagerModel();

            dataManager = new DataManager();

            detailsPageView = new DetailsPage();
            detailsPageViewModel = new DetailsPageViewModel();

            RefreshDataBaseCommand = new Command(async () => await LoadPokemonList(pkmToFind));

            PerformSearchCommand = new Command<string>((string query) => { SearchSuggestionsCollection = GetSearchResults(query); });

            ItemTappedCommand = new Command<PokedexModel>(async p => await ItemTapped(p));

            ClearDataCommand = new Command(ClearData);

            RefreshDataBaseCommand.Execute(null);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Command RefreshDataBaseCommand { get; }

        public Command<PokedexModel> ItemTappedCommand { get; }

        public Command ClearDataCommand { get; }

        public Command<string> PerformSearchCommand { get; }

        public ObservableCollection<PokedexModel> pkmList { get; private set; }

        public ObservableCollection<PokedexModel> SearchSuggestionsCollection
        {
            get
            {
                return searchSuggestionsCollection;
            }
            set
            {
                searchSuggestionsCollection = value;
                OnPropertChanged("SearchSuggestionsCollection");
            }
        }

        public string SearchBoxPlaceHolder { get; private set; } = "Search";

        public string Logo_Header => logoPath;
        public bool IsSearchResultsListVisible
        {
            get
            {
                return isSearchResultsListVisible;
            }
            set
            {
                isSearchResultsListVisible = value;
                OnPropertChanged("IsSearchResultsListVisible");
            }
        }

        private void OnPropertChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        private ObservableCollection<PokedexModel> GetSearchResults(string query)
        {
            if (query.Length >= 1)
            {
                IsSearchResultsListVisible = true;
                //return new ObservableCollection<PokedexModel>(pkmList.Where(p => p.Name.ToLower().StartsWith(query.ToLower())));
                return new ObservableCollection<PokedexModel>(pkmList.Where(p => p.Name.ToLower().Contains(query.ToLower())));

            }
            else
            {
                IsSearchResultsListVisible = false;
                return null;
            }
        }

        private async Task LoadPokemonList(int pkmToFind)
        {
            pkmList = new ObservableCollection<PokedexModel>(await dataManager.LoadPokemonDataList(pkmToFind));
            OnPropertChanged(null);
        }

        private async Task ItemTapped(PokedexModel pkm)
        {
            MessagingCenter.Send<MainPageViewModel, PokedexModel>(this, "Send_Selected_Pokemon", pkm);
            detailsPageView.BindingContext = detailsPageViewModel;
            await Application.Current.MainPage.Navigation.PushAsync(detailsPageView);
        }      

        private void ClearData()
        {
            dataManager.RemovePokemonDataFile();
            RefreshDataBaseCommand.Execute(null);
        }
    }
}
