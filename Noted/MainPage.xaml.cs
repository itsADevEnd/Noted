using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Noted
{
    public partial class MainPage : ContentPage
    {
        /// <summary>
        /// The singleton used to interact with and manipulate this page's content. This is also the property that is being set as the root page of the application.
        /// </summary>
        public static MainPage AppMainPage { get; set; }
        /// <summary>
        /// Used to store the note names and their content during runtime to keep track of the data.
        /// </summary>
        public Dictionary<string, string> NameContentNotes = new Dictionary<string, string>();

        public MainPage()
        {
            InitializeComponent();
            NotedDatabase.InitializeConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Noted.db3"));
            GetNotes();
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
                    result = result.Trim();

                    if (NameContentNotes.ContainsKey(result))
                    {
                        await DisplayAlert("Cannot Use Note Name", $"The note name '{result}' is already being used with another note. Please use another note name.", "OK");
                    }
                    else
                    {
                        noteNameIsEmpty = false;
                    }
                }
            } while (noteNameIsEmpty == true);


            EditNote editNotePage = new EditNote(result);
            await Navigation.PushModalAsync(editNotePage);
        }

        public void NoteCell_Tapped(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new EditNote(sender as TextCell));
        }

        private async void GetNotes()
        {
            List<NotedModel> notes = await NotedDatabase.GetNotesAsync();

            if (notes.Count > 0)
            {
                foreach (NotedModel note in notes)
                {
                    TextCell noteTextCell = new TextCell()
                    {
                        Text = note.NoteName,
                        Detail = note.Note,
                        TextColor = Color.FromHex("#2196F3"),
                        DetailColor = Color.FromHex("#2196F3"),
                    };
                    noteTextCell.Tapped += NoteCell_Tapped;

                    AppMainPage.TextCellContainer.Add(noteTextCell);
                    NameContentNotes.Add(note.NoteName, note.Note);
                }
            }
        }
    }
}