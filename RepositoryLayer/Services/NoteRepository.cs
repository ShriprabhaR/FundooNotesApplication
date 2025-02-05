using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using CommonLayer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;
using RepositoryLayer.Migrations;

namespace RepositoryLayer.Services
{
    public class NoteRepository : INotesRepository
    {
        private readonly FunDooDBContext context;
        private readonly IConfiguration configuration;

        public NoteRepository(FunDooDBContext context, IConfiguration configuration)
        {
            this.context = context;
            this.configuration = configuration;
        }

        public Notes AddNote(int UserId, NoteModel model)
        {
            Notes notes = new Notes();
            notes.Title = model.Title;
            notes.Description = model.Description;
            notes.UserId = UserId;
            notes.CreatedAt = DateTime.Now;
          
            context.Add(notes);
            context.SaveChanges();
            return notes;

        }

        public List<Notes> GetNotes(int UserId)
        {
            var listOfNotes = context.Notes.Where(x => x.UserId == UserId).ToList();
            return listOfNotes;
        }


        public Notes UpdateNotes(int UserId, int NotesId, UpdateNotesModel model)
        {
            var notes = context.Notes.FirstOrDefault(x => x.UserId == UserId && x.NotesId == NotesId);

            if (notes == null)
            {
                return null;
            }

            notes.Title = model.Title;
            notes.Description = model.Description;
            notes.Reminder = model.Reminder;
            notes.Color = model.Color;
            notes.IsPin = model.IsPin;
            notes.IsArchive = model.IsArchive;
            notes.IsTrash = model.IsTrash;
            notes.UpdatedAt = DateTime.Now;

            context.SaveChanges();

            return notes;
        }

        public bool DeleteNotes(int NotesId, int UserId)
        {
            var notes = context.Notes.FirstOrDefault(x => x.NotesId == NotesId && x.UserId == UserId);
            if (notes == null)
            {
                return false;
            }
            context.Remove(notes);
            context.SaveChanges();
            return true;
        }


        public bool IsPinOrUnpin(int UserId, int NotesId)
        {
            var check = context.Notes.FirstOrDefault(x => x.UserId == UserId && x.NotesId == NotesId);

            if (check != null)
            {
                if (check.IsPin == true)
                {
                    check.IsPin = false;
                    context.SaveChanges();
                    return true;
                }
                else
                {
                    check.IsPin = true;
                    context.SaveChanges();
                    return true;
                }
            }
            else
            {
                return false;
            }
            
        }

        public bool IsArchiveOrUnArchive(int UserId, int NotesId)
        {
            var check = context.Notes.FirstOrDefault(x => x.UserId == UserId && x.NotesId == NotesId);

            if (check != null)
            {
                if (check.IsArchive == true)
                {
                    check.IsArchive = false;
                    context.SaveChanges();
                    return true;
                }
                else
                {
                    check.IsArchive = true;
                    context.SaveChanges();
                    return true;
                }
            }
            else
            {
                return false;
            }

        }

        public bool IsTrashOrUnTrash(int UserId, int NotesId)
        {
            var check = context.Notes.FirstOrDefault(x => x.UserId == UserId && x.NotesId == NotesId);

            if (check != null)
            {
                if (check.IsTrash == true)
                {
                    check.IsTrash = false;
                    context.SaveChanges();
                    return true;
                }
                else
                {
                    check.IsTrash = true;
                    context.SaveChanges();
                    return true;
                }
            }
            else
            {
                return false;
            }

        }

        public Notes DeleteForever(int UserId, int NotesId)
        {
            try
            {
                var checkExist = context.Notes.SingleOrDefault(x => x.UserId == UserId && x.NotesId == NotesId);
                if (checkExist != null)
                {
                    context.Notes.Remove(checkExist);
                    context.SaveChanges();
                    return checkExist;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
           

        }

        public Notes SetRemainder(int userId, int notesId, DateTime reminderTime)
        {
            try
            {
                var note = context.Notes.FirstOrDefault(x => x.UserId == userId && x.NotesId == notesId);

                if (note != null)
                {
                    note.Reminder = reminderTime;
                    context.SaveChanges();
                    return note;
                }

                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Notes Color(int UserId, int NotesId, string color)
        {
            try
            {
                var check = context.Notes.SingleOrDefault(x => x.UserId == UserId && x.NotesId == NotesId);
                if (check != null)
                {
                    check.Color = color;
                    context.SaveChanges();
                    return check;

                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public Notes getNotesById(int UserId, int NotesId)
        {
            var check = context.Notes.SingleOrDefault(x => x.UserId == UserId && x.NotesId == NotesId);
            if(check != null)
            {
                return check;
            }
            return null;

        }

        public string UploadImage(IFormFile ImagePath, int NotesId, int UserId)
        {
                var user = context.Users.Any(x => x.UserId == UserId);
                if (user)
                {
                    var note = getNotesById(UserId, NotesId);
                    if (note != null)
                    {
                        Account account = new Account(configuration["Cloudinary:CloudName"], configuration["Cloudinary:ApiKey"], configuration["Cloudinary:ApiSecret"]);
                        Cloudinary cloudinary = new Cloudinary(account);
                        var uploadParams = new ImageUploadParams()
                        {
                            File = new FileDescription(ImagePath.FileName, ImagePath.OpenReadStream()),
                            PublicId = note.Title

                        };
                        var uploadImages = cloudinary.Upload(uploadParams);
                        if (uploadParams != null)
                        {
                            note.UpdatedAt = DateTime.Now;
                            note.Image = uploadImages.Url.ToString();
                            context.SaveChanges();
                            return uploadImages.Url.ToString();
                        }
                        else throw new Exception("Image not uploaded for note id: " + NotesId);

                    }
                    else throw new Exception("Note not found for requested note id: " + NotesId);
                }
                else throw new Exception("User not found for requested user id: " + UserId);
            
        }

    }
}
