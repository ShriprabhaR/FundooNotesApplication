using System;
using System.Collections.Generic;
using System.Linq;
using CommonLayer.Models;
using ManagerLayer.Interface;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Migrations;

namespace FundooNotesApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private readonly INoteManager manager;
        private readonly FunDooDBContext context;

        public NotesController(INoteManager manager)
        {
            this.manager = manager;
        }

        [Authorize]
        [HttpPost("AddNote")]

        public IActionResult AddNotes(NoteModel model)
        {
            string data = User.Claims.FirstOrDefault(c => c.Type == "UserId").Value;
            var response = manager.AddNote(Convert.ToInt32(data), model);

            if (response != null)
            {
                return Ok(new ResponseModel<Notes> { Success = true, Message = "Notes added", Data = response});
            }
            else
            {
                return BadRequest(new ResponseModel<Notes> { Success = false, Message = "Notes not added" });
            }
        }

        [Authorize]
        [HttpGet("GetAllNotes")]
        public IActionResult GetNotes()
        {
            string data = User.Claims.FirstOrDefault(c => c.Type == "UserId").Value;
            var response = manager.GetNotes(Convert.ToInt32(data));

            if (response != null)
            {
                return Ok(new ResponseModel<List<Notes>> { Success = true, Message = "Notes retrieved", Data = response });
            }
            else
            {
                return BadRequest(new ResponseModel<List<Notes>> { Success = false, Message = "unable to retrieve" });
            }

        }

        [Authorize]
        [HttpPut("UpdateNotes")]
        public IActionResult UpdateNotes(UpdateNotesModel model)
        {
            var userClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");

            if (userClaim == null)
            {
                return Unauthorized(new ResponseModel<Notes> { Success = false, Message = "Invalid user authentication" });
            }
            int userId = Convert.ToInt32(userClaim.Value);
            var allNotes = manager.GetNotes(userId);

            var updatedNote = manager.UpdateNotes(Convert.ToInt32(userId), model.NotesId, model);

            if (updatedNote != null)
            {
                return Ok(new ResponseModel<Notes>{Success = true,Message = "Note updated successfully",Data = updatedNote});
            }
            else
            {
                return NotFound(new ResponseModel<Notes>{Success = false,Message = "Note not found or unauthorized"});
            }
        }

        [Authorize]
        [HttpDelete("DeleteNotes")]
        public IActionResult DeleteNotes(int NotesId)
        {
            var userClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");

            if (userClaim == null)
            {
                return Unauthorized(new ResponseModel<Notes> { Success = false, Message = "Invalid user authentication" });
            }

            int userId = Convert.ToInt32(userClaim.Value);
            var result = manager.DeleteNotes(NotesId, userId);

            if (result)
            {
                return Ok(new ResponseModel<bool> { Success = true, Message = "Note deleted successfully" });
            }
            else
            {
                return NotFound(new ResponseModel<bool> { Success = false, Message = "Note not found or unauthorized" });
            }
        }




        //Fetch Notes using title and description
        [HttpGet("GetNotes")]
        public IActionResult GetNotes(string title, string description)
        {
            var notes = context.Notes.Select(x => x).Where(x => x.Title == title && x.Description == description).ToList();

            if(notes != null)
            {
                return Ok(new ResponseModel<List<Notes>>{Success = true,Message = "Notes retrieved successfully",Data = notes});

            }
            else 
            {
                return BadRequest(new ResponseModel<List<Notes>>{Success = false,Message = "No notes found"});
            }
        }

        //Return Count of notes a user has
        [HttpGet("CountNotes")]
        public IActionResult GetUserNotesCount(int userId)
        {
            var notesCount = context.Notes.Where(x => x.UserId == userId).Count();

            if (notesCount > 0)
            {
                return Ok(new ResponseModel<int>{Success = true,Message = "Notes count retrieved successfully",Data = notesCount});
            }
            else
            {
                return BadRequest(new ResponseModel<int> { Success = false, Message = "Notes not found" });
            }
        }



    }
}
