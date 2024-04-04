using PassIn.Communication.Responses;
using PassIn.Infrastructure;
using PassIn.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace PassIn.Application.UseCases.Attendees.GetAllByEventsId
{
    public class GetAllAttendeesByEventIdUseCase
    {
        private readonly PassInDbContext _dbContext;

        public GetAllAttendeesByEventIdUseCase()
        {
            _dbContext = new PassInDbContext();
        }

        public ResponseAllAttendeesJson Execute(Guid eventId)
        {
            Validate(eventId);

            ResponseAllAttendeesJson allAtendees = new ResponseAllAttendeesJson
            {
                Attendees = _dbContext.Events
                .Include(ev => ev.Attendees)
                .ThenInclude(att => att.CheckIn)
                .FirstOrDefault(ev => ev.Id == eventId)
                .Attendees
                .Select(at => new ResponseAttendeeJson
                {
                    Id = at.Id,
                    Name = at.Name,
                    Email = at.Email,
                    CreatedAt = at.Created_At,
                    CheckedInAt = at.CheckIn?.Created_at,
                })
                .ToList()
            };
            return allAtendees;
        }

        private void Validate(Guid eventId)
        {
            var eventEntity = _dbContext.Events
                .Include(ev => ev.Attendees)
                .ThenInclude(att => att.CheckIn)
                .FirstOrDefault(ev => ev.Id == eventId);
            if (eventEntity is null)
                throw new NotFoundException("No events were found with this id.");

            if (!eventEntity.Attendees.Any())
                throw new NotFoundException("No attendees were found for this event.");
        }
    }
}
