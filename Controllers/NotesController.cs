using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLayer.Models;
using FundooNotesApp.Helper;
using GreenPipes.Caching;
using ManagerLayer.Interface;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Migrations;

namespace FundooNotesApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private readonly IDistributedCache distributedCache;
        private readonly INoteManager manager;
        private readonly FunDooDBContext context;
        private readonly ILogger<NotesController> logger;

        public NotesController(INoteManager manager, FunDooDBContext context, ILogger<NotesController> logger, IDistributedCache distributedCache)
        {
            this.manager = manager;
            this.context = context;
            this.logger = logger;
            this.distributedCache = distributedCache;
        }

        [Authorize]
        [HttpPost("AddNote")]

        public IActionResult AddNotes(NoteModel model)
        {
            string data = User.Claims.FirstOrDefault(c => c.Type == "UserId").Value;
            var response = manager.AddNote(Convert.ToInt32(data), model);

            if (response != null)
            {
                return Ok(new ResponseModel<Notes> { Success = true, Message = "Notes added", Data = response });
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
                return Ok(new ResponseModel<Notes> { Success = true, Message = "Note updated successfully", Data = updatedNote });
            }
            else
            {
                return NotFound(new ResponseModel<Notes> { Success = false, Message = "Note not found or unauthorized" });
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
            var notes = context.Notes.Select(x => x).Where(x => x.Title == title && x.Description == description);

            if (notes != null)
            {
                return Ok(new ResponseModel<List<Notes>> { Success = true, Message = "Notes retrieved successfully", Data = notes.ToList() });

            }
            else
            {
                return BadRequest(new ResponseModel<List<Notes>> { Success = false, Message = "No notes found" });
            }
        }

        //Return Count of notes a user has
        [HttpGet("CountNotes")]
        public IActionResult GetUserNotesCount(int userId)
        {
            var notesCount = context.Notes.Where(x => x.UserId == userId).Count();

            if (notesCount > 0)
            {
                return Ok(new ResponseModel<int> { Success = true, Message = "Notes count retrieved successfully", Data = notesCount });
            }
            else
            {
                return BadRequest(new ResponseModel<int> { Success = false, Message = "Notes not found" });
            }
        }


        [Authorize]
        [HttpPut("IsPinUnpin")]
        public IActionResult IsPinOrUnpin(int NotesId)
        {
            try { 
            string data = User.Claims.FirstOrDefault(c => c.Type == "UserId").Value;
            var response = manager.IsPinOrUnpin(Convert.ToInt32(data), NotesId);

                logger.LogInformation(response.ToString());
                return Ok(new ResponseModel<bool> { Success = true, Message = "Pin/Unpin Updated", Data = response });
            }
            catch(AppException ex)
            {
                logger.LogError(ex.ToString());
                return BadRequest(new ResponseModel<bool> { Success = false, Message = "Pin/Unpin Not Updated" });
            }

        }

        [Authorize]
        [HttpPut("IsArchiveOrUnArchive")]
        public IActionResult IsArchiveOrUnArchive(int NotesId)
        {
            try
            {
                string data = User.Claims.FirstOrDefault(c => c.Type == "UserId").Value;
                var response = manager.IsArchiveOrUnArchive(Convert.ToInt32(data), NotesId);
                logger.LogInformation(response.ToString());
                return Ok(new ResponseModel<bool> { Success = true, Message = "Archive/UnArchive Updated", Data = response });
                
            }
            catch(AppException ex)
            {
                logger.LogError(ex.ToString());
                return BadRequest(new ResponseModel<bool> { Success = false, Message = "Archive/UnArchive Not Updated" });
            }

        }

        [Authorize]
        [HttpPut("IsTrashOrUnTrash")]
        public IActionResult IsTrashOrUnTrash(int NotesId)
        {
            try
            {
                string data = User.Claims.FirstOrDefault(c => c.Type == "UserId").Value;
                var response = manager.IsTrashOrUnTrash(Convert.ToInt32(data), NotesId);
                logger.LogInformation(response.ToString());
                return Ok(new ResponseModel<bool> { Success = true, Message = "Trash/UnTrash Updated", Data = response });
                
            }
            catch (AppException ex)
            {
                logger.LogError(ex.ToString());
                return BadRequest(new ResponseModel<bool> { Success = false, Message = "Trash/UnTrash Not Updated" });
            }

        }

        [Authorize]
        [HttpPut("Delete forever")]

        public IActionResult DeleteForever(int NotesId)
        {
            try
            {
                string data = User.Claims.FirstOrDefault(c => c.Type == "UserId").Value;
                var response = manager.DeleteForever(Convert.ToInt32(data), NotesId);

                logger.LogInformation(response.ToString());
                return Ok(new ResponseModel<Notes> { Success = true, Message = "date Deleted Permanently", Data = response });
            }
            catch (AppException ex)
            {

                logger.LogError(ex.ToString());
                return BadRequest(new ResponseModel<Notes> { Success = false, Message = "User/ Note not found" });
            }
            
        }

        [Authorize]
        [HttpPut("Reminder")]

        public IActionResult ManageRemainder(int NotesId, DateTime reminderTime)
        {
            try
            {
                string data = User.Claims.FirstOrDefault(c => c.Type == "UserId").Value;
                var response = manager.SetRemainder(Convert.ToInt32(data), NotesId, reminderTime);
                
                logger.LogInformation(response.ToString());
                return Ok(new ResponseModel<Notes> { Success = true, Message = "Remainder set", Data = response });
                
            }
            catch (AppException ex)
            {
                logger.LogError(ex.ToString());
                return BadRequest(new ResponseModel<Notes> { Success = false, Message = "Unable to find user/notes" });
            }
        }
        [Authorize]
        [HttpPut("Color")]

        public IActionResult Color(int NotesId, string color)
        {
            try
            {
                string data = User.Claims.FirstOrDefault(c => c.Type == "UserId").Value;
                var response = manager.Color(Convert.ToInt32(data), NotesId, color);
                logger.LogInformation(response.ToString());
                    return Ok(new ResponseModel<Notes> { Success = true, Message = "Set color", Data = response });
                
            }
            catch (AppException ex)
            {
                logger.LogError(ex.ToString());
                return BadRequest(new ResponseModel<Notes> { Success = false, Message = "Unable to find user/notes" });
            }
        }

        [Authorize]
        [HttpPut("UploadImage")]

        public IActionResult Image(int NotesId, IFormFile ImagePath)
        {
            try
            {
                string data = User.Claims.FirstOrDefault(c => c.Type == "UserId").Value;
                var response = manager.UploadImage(ImagePath, NotesId, Convert.ToInt32(data));

                logger.LogInformation(response.ToString()); 
                return Ok(new ResponseModel<string> { Success = true, Message = "Image updated", Data = response });
            }
            catch (AppException ex)
            {
                logger.LogError(ex.ToString());
                return BadRequest(new ResponseModel<string> { Success = false, Message = "Unable to find user/notes" });
            }
            
        }

        
        [HttpGet("UsingRedisCache")]
        
        public async Task<IActionResult> GetAllNotesUsingRedisCache()
        {
            var cacheKey = "NotesList";
            string SerializedNotesLst;
            var NotesList = new List<Notes>();
            var RedisNotesList = await distributedCache.GetAsync(cacheKey);
            if (RedisNotesList != null)
            {
                SerializedNotesLst = Encoding.UTF8.GetString(RedisNotesList);
                NotesList = JsonConvert.DeserializeObject<List<Notes>>(SerializedNotesLst);
            }

            else
            {
                NotesList = context.Notes.ToList();
                SerializedNotesLst = JsonConvert.SerializeObject(NotesList);
                RedisNotesList = Encoding.UTF8.GetBytes(SerializedNotesLst);
                var options = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(DateTime.Now.AddMinutes(25))
                    .SetSlidingExpiration(TimeSpan.FromSeconds(5));
                await distributedCache.SetAsync(cacheKey, RedisNotesList, options);
            }
            return Ok(NotesList);
              
        }

    }
}
