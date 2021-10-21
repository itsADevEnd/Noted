using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Noted
{
    public partial class MainPage : ContentPage
    {
        public static MainPage AppMainPage { get; set; }
        public ObservableCollection<string> Notes { get; set; } = new ObservableCollection<string>();
        public Dictionary<string, string> NameContentNotes = new Dictionary<string, string>();
        public string Note { get; set; }
        public string TemporaryNoteName { get; set; }

        public MainPage()
        {
            InitializeComponent();
            NotesListView.ItemsSource = Notes;
        }

        private void AddNote_Clicked(object sender, EventArgs e)
        {
            NewNote();
        }

        private async void NewNote()
        {
            string result;
            bool noteNameIsEmpty = false;

            do
            {
                result = await DisplayPromptAsync("Name Your Note", "Enter the name of your note", accept: "Continue", maxLength: 50);

                if (result == null) return;
                else if (string.IsNullOrWhiteSpace(result))
                {
                    noteNameIsEmpty = true;
                    await DisplayAlert("Note Name is Empty", "The name of your note cannot be empty. Please try again.", "OK");
                }
                else
                {
                    noteNameIsEmpty = false;
                }
            } while (noteNameIsEmpty == true);


            TemporaryNoteName = result;
            Page editNotePage = new EditNote(string.Empty);
            await Navigation.PushModalAsync(editNotePage);
        }

        private async void NotesListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            await Navigation.PushModalAsync(new EditNote((sender as ListView).SelectedItem.ToString(), e.ItemIndex));
            NotesListView.SelectedItem = null;
        }
    }
}