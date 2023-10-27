using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotesWebApi.Models;

namespace NotesWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private readonly NoteContext _context;

        public NotesController(NoteContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all user notes
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <response code="404">There is not a note with this id</response>
        [HttpGet("user/{userId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<IEnumerable<NoteDTO>>> GetNotes(int userId)
        {
            var notes = await _context.Notes
                .Where(n => n.UserId == userId)
                .Select(x => new NoteDTO
                {
                    Id = x.Id,
                    Title = x.Title,
                    Body = x.Body,
                    UpdatedOn = x.UpdatedOn,
                    CreatedOn = x.CreatedOn,
                    UserId = x.UserId
                })
                .ToListAsync();

            if (notes == null)
            {
                return NotFound();
            }

            return notes;
        }

        /// <summary>
        /// Gets a specific note
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="404">There is not a note with this id</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<NoteDTO>> GetNote(int id)
        {
            var note = await _context.Notes.FindAsync(id);

            if (note == null)
            {
                return NotFound();
            }

            return NoteToDTO(note);
        }

        /// <summary>
        /// Edites a specific note
        /// </summary>
        /// <param name="id"></param>
        /// <param name="noteDTO"></param>
        /// <returns></returns>
        /// <response code="204">Editing was successful</response>
        /// <response code="400">Id in parameter and id in don't match</response>
        /// <response code="404">There is not a note with this id</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<IActionResult> PutNote(int id, NoteDTO noteDTO)
        {
            if (id != noteDTO.Id)
            {
                return BadRequest();
            }

            var note = await _context.Notes.FindAsync(id);
            if (note == null)
            {
                return NotFound();
            }

            note.Title = noteDTO.Title;
            note.Body = noteDTO.Body;
            note.UpdatedOn = DateTime.Now;
            note.UserId = noteDTO.UserId;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!NoteExists(id))
            {
                return NotFound();
            }

            return NoContent();
        }

        /// <summary>
        /// Creates new note
        /// </summary>
        /// <param name="noteDTO"></param>
        /// <returns></returns>
        /// <response code="201">Returns the newly created item</response>
        /// <response code="400">If the item is null</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<NoteDTO>> PostNote(NoteDTO noteDTO)
        {
            var note = new Note
            {
                Title = noteDTO.Title,
                Body = noteDTO.Body,
                CreatedOn = DateTime.Now,
                UpdatedOn = DateTime.Now,
                UserId = noteDTO.UserId
            };

            _context.Notes.Add(note);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetNote),
                new { id = note.Id },
                NoteToDTO(note));
        }

        /// <summary>
        /// Deletes a specific note
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="204">Deleting was successful</response>
        /// <response code="404">There is not a note with this id</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteNote(int id)
        {
            var note = await _context.Notes.FindAsync(id);
            if (note == null)
            {
                return NotFound();
            }

            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool NoteExists(int id)
        {
            return (_context.Notes?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private static NoteDTO NoteToDTO(Note note) => new NoteDTO
        {
           Id = note.Id,
           Title = note.Title,
           Body = note.Body,
           CreatedOn = DateTime.Now,
           UpdatedOn = DateTime.Now,
           UserId = note.UserId,
       };
    }
}
