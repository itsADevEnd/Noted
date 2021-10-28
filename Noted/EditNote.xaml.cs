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
        private string temporaryNoteContent = "";
        private string noteName = "";
        private TextCell noteTextCell;
        private bool isNewNote = false;

        /// <summary>
        /// To be used when an existing note is being opened.
        /// </summary>
        /// <param name="textCell">The TextCell that contains the Text and Detail content.</param>
        /// <param name="noteItemIndex"></param>
        public EditNote(TextCell textCell)
        {
            InitializeComponent();
            noteTextCell = textCell;
            noteName = textCell.Text;
            PopulateNoteEditor();
            FocusOnTextEditor();
        }

        public EditNote(string nameOfNote)
        {
            InitializeComponent();
            noteName = nameOfNote;
            isNewNote = true;
            FocusOnTextEditor();
        }

        /// <summary>
        /// Sets the Text property of the note editor with the content of the note if a record with the note's name can be found.
        /// </summary>
        private void PopulateNoteEditor()
        {
            if (mainPage.NameContentNotes.TryGetValue(noteTextCell.Text, out string noteContent))
            {
                NoteEditor.Text = noteContent;
                NoteEditor.IsEnabled = false;
            }
            else
            {
                DisplayAlert("Note Not Found", $"Note could not be found.", "Return to Notes");
                Navigation.PopModalAsync();
            }
        }

        private void FocusOnTextEditor()
        {
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
                NotedModel note = new NotedModel(noteName, NoteEditor.Text);

                if (isNewNote)
                {
                    if (await NotedDatabase.SaveNoteAsync(note) > 0)
                    {
                        mainPage.NameContentNotes.Add(noteName, NoteEditor.Text);
                        TextCell noteTextCell = new TextCell()
                        {
                            Text = noteName,
                            Detail = NoteEditor.Text,
                            TextColor = Color.FromHex("#2196F3"),
                            DetailColor = Color.FromHex("#2196F3"),
                        };
                        noteTextCell.Tapped += mainPage.NoteCell_Tapped;
                        MainPage.AppMainPage.TextCellContainer.Add(noteTextCell);
                    }
                    else
                    {
                        // Add code here to handle any errors that occur when trying to save a note.
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

                noteName = string.Empty;
                isNewNote = false;
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
                    //mainPage.NoteNames.Remove(noteTextCell);
                    mainPage.TextCellContainer.Remove(noteTextCell);
                    await Navigation.PopModalAsync();
                }
            }
        }
    }
}