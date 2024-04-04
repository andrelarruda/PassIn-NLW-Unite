using PassIn.Communication.Responses;
using PassIn.Exceptions;
using PassIn.Infrastructure;
using PassIn.Infrastructure.Entities;

namespace PassIn.Application.UseCases.Checkins.DoCheckin
{
    public class DoAttendeeCheckinUseCase
    {
        private readonly PassInDbContext _dbContext;

        public DoAttendeeCheckinUseCase()
        {
            _dbContext = new PassInDbContext();
        }

        public ResponseRegisteredJson Execute(Guid attendeeId)
        {
            Validate(attendeeId);

            var entity = new CheckIn
            {
                Attendee_Id = attendeeId,
            };
            
            _dbContext.CheckIns.Add(entity);
            _dbContext.SaveChanges();
            return new ResponseRegisteredJson()
            {
                Id = entity.Id,
            };
        }

        private void Validate(Guid attendeeId)
        {
            var existAttendee = _dbContext.Attendees.Any(att => att.Id == attendeeId);
            if (existAttendee == false)
                throw new NotFoundException("The attendee with this id was not found.");

            var existCheckin = _dbContext.CheckIns.Any(checkin => checkin.Attendee_Id == attendeeId);
            if (existCheckin)
                throw new ConflictException("Attendee cannot do checking twice in the same event.");
        }
    }
}
