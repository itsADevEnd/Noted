using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Noted
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditNote : ContentPage
    {
        private MainPage mainPage = MainPage.AppMainPage;
        private int itemIndex = -1;
        private string temporaryNoteContent = "";
        private string noteName = "";

        public EditNote(object note, int noteItemIndex = -1)
        {
            InitializeComponent();
            if (!string.IsNullOrEmpty(note.ToString()))
            {
                if (mainPage.NameContentNotes.TryGetValue(note.ToString(), out string noteContent))
                {
                    noteName = note.ToString();
                    NoteEditor.Text = noteContent;
                    NoteEditor.IsEnabled = false;
                }
                else
                {
                    DisplayAlert("Note Not Found", $"The note titled {noteName} could not be found.", "Return to Notes");
                    Navigation.PopModalAsync();
                }
            }
            else
            {
                EditNoteButton.IsVisible = false;
            }
            itemIndex = noteItemIndex;
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 500;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            NoteEditor.Focus();
            (sender as System.Timers.Timer).Stop();
        }

        private async void SaveNote_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NoteEditor.Text))
            {
                await DisplayAlert("Note is Empty", "Note cannot be empty. Please type out a note.", "OK");
                return;
            }
            else if (itemIndex == -1)
            {
                mainPage.Notes.Add(mainPage.TemporaryNoteName);
                mainPage.NameContentNotes.Add(mainPage.TemporaryNoteName, NoteEditor.Text);
                mainPage.TemporaryNoteName = string.Empty;
            }
            else
            {
                if (mainPage.NameContentNotes.TryGetValue(noteName, out string noteContent))
                {
                    mainPage.NameContentNotes[noteName] = noteContent;
                }
                else
                {
                    await DisplayAlert("Note Not Found", $"The note titled {noteName} could not be found. Unable to save.", "Return to Notes");
                }
            }
            await Navigation.PopModalAsync();
        }

        private void EditNote_Clicked(object sender, EventArgs e)
        {
            NoteEdit(sender as Button);
        }

        private void NoteEdit(Button editButton)
        {
            if (editButton.Text == "Edit Note")
            {
                temporaryNoteContent = NoteEditor.Text;
                editButton.Text = "Cancel Editing";
                NoteEditor.IsEnabled = true;
            }
            else if (editButton.Text == "Cancel Editing")
            {
                NoteEditor.Text = temporaryNoteContent;
                editButton.Text = "Edit Note";
                NoteEditor.IsEnabled = false;
            }
        }

        private async void ReturnToNotes_Clicked(object sender, EventArgs e)
        {
            bool returnToNotes = await DisplayAlert("Return to Notes", "Are you sure you want to return to your notes? All unsaved changes will be lost.", "Yes", "No");

            if (returnToNotes) await Navigation.PopModalAsync();
        }
    }
}