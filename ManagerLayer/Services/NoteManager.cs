using System;
using System.Collections.Generic;
using System.Text;
using CommonLayer.Models;
using ManagerLayer.Interface;
using Microsoft.AspNetCore.Http;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;

namespace ManagerLayer.Services
{
    public class NoteManager : INoteManager
    {
        private readonly INotesRepository note;
        private readonly FunDooDBContext context;

        public NoteManager(INotesRepository note, FunDooDBContext context)
        {
            this.note = note;
            this.context = context;
        }

        public Notes AddNote(int UserId, NoteModel model)
        {
            return note.AddNote(UserId, model);
        }

        public List<Notes> GetNotes(int UserId)
        {
            return note.GetNotes(UserId);
        }

        public Notes UpdateNotes(int UserId, int NotesId, UpdateNotesModel model)
        {
            return note.UpdateNotes( UserId, NotesId, model);
        }

        public bool DeleteNotes(int NotesId, int UserId)
        {
            return note.DeleteNotes( NotesId, UserId);
        }

        public bool IsPinOrUnpin(int UserId, int NotesId)
        {
            return note.IsPinOrUnpin(UserId, NotesId);
        }

        public bool IsArchiveOrUnArchive(int UserId, int NotesId)
        {
            return note.IsArchiveOrUnArchive(UserId, NotesId);
        }
        public bool IsTrashOrUnTrash(int UserId, int NotesId)
        {
            return note.IsTrashOrUnTrash(UserId, NotesId);
        }

        public Notes DeleteForever(int UserId, int NotesId)
        {
            return note.DeleteForever( UserId, NotesId);
        }
        public Notes SetRemainder(int userId, int notesId, DateTime reminderTime)
        {
            return note.SetRemainder( userId, notesId, reminderTime);
        }
        public Notes Color(int UserId, int NotesId, string color)
        {
            return note.Color( UserId, NotesId, color);
        }
        public string UploadImage(IFormFile ImagePath, int NotesId, int UserId)
        {
            return note.UploadImage(ImagePath, NotesId, UserId);
        }
    }
}
