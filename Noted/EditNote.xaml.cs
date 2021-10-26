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
                    DisplayAlert("Note Not Found", $"Note could not be found.", "Return to Notes");
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
            else
            {
                NotedModel note = new NotedModel(mainPage.TemporaryNoteName, NoteEditor.Text);

                if (itemIndex == -1)
                {
                    if (await NotedDatabase.SaveNoteAsync(note) > 0)
                    {
                        mainPage.NoteNames.Add(mainPage.TemporaryNoteName);
                        mainPage.NameContentNotes.Add(mainPage.TemporaryNoteName, NoteEditor.Text);
                        mainPage.TemporaryNoteName = string.Empty;
                    }
                    else
                    {
                        // Add code here to handle any errors that occur when trying to save a note
                    }
                }
                else
                {
                    if (await NotedDatabase.UpdateNoteAsync(note) > 0) mainPage.NameContentNotes[noteName] = NoteEditor.Text;
                    else
                    {
                        await DisplayAlert("Unable to Update Note", "The note could not be updated.", "OK");
                        return;
                    }
                }
                await Navigation.PopModalAsync();
            }
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

        private async void DeleteNote_Clicked(object sender, EventArgs e)
        {
            if (await DisplayAlert("Delete Note", "Are you sure you want to delete this note? This action cannot be reversed.", "Yes", "No"))
            {
                if (await NotedDatabase.DeleteNoteAsync(new NotedModel(noteName, NoteEditor.Text)) > 0)
                {
                    mainPage.NameContentNotes.Remove(noteName);
                    mainPage.NoteNames.Remove(noteName);
                    await Navigation.PopModalAsync();
                }
            }
        }
    }
}