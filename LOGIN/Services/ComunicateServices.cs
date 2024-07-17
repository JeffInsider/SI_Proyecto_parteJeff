using AutoMapper;
using LOGIN.Dtos;
using LOGIN.Dtos.Communicates;
using LOGIN.Entities;
using LOGIN.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LOGIN.Services
{
    public class ComunicateServices : IComunicateServices
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly HttpContext _httpContext;
        private readonly string _USER_ID;

        public ComunicateServices(ApplicationDbContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _httpContext = httpContextAccessor.HttpContext;
            var idClaim = _httpContext.User.Claims.Where(x => x.Type == "UserId")
                .FirstOrDefault();
            _USER_ID = idClaim?.Value;
        }


        public async Task<ResponseDto<CommunicateDto>> CreateCommunicate(CreateCommunicateDto model)
        {
            var communicateEntity = _mapper.Map<CommunicateEntity>(model);
            communicateEntity.Date = DateTime.UtcNow;

            //solo para permitir al usuario que esta logueado crear un comunicado
            communicateEntity.User_Id = _USER_ID;


            _dbContext.Communicates.Add(communicateEntity);
            await _dbContext.SaveChangesAsync();

            var communicateDto = _mapper.Map<CommunicateDto>(communicateEntity);

            return new ResponseDto<CommunicateDto>
            {
                Status = true,
                StatusCode = 201,
                Message = "Comunicado creado correctamente",
                Data = communicateDto
            };
        }

        //traer todos los comunicados
        public async Task<ResponseDto<List<CommunicateDto>>> GetAllCommunicates()
        {
            var communicates = await _dbContext.Communicates.ToListAsync();
            var communicatesDto = _mapper.Map<List<CommunicateDto>>(communicates);

            return new ResponseDto<List<CommunicateDto>>
            {
                Status = true,
                StatusCode = 200,
                Message = "Lista de comunicados",
                Data = communicatesDto
            };
        }

        public async Task<ResponseDto<CommunicateDto>> UpdateCommunicate(CommunicateDto model)
        {
            var communicateEntity = await _dbContext.Communicates.FirstOrDefaultAsync(x => x.Id == model.Id);

            if (communicateEntity == null)
            {
                return new ResponseDto<CommunicateDto>
                {
                    Status = false,
                    StatusCode = 404,
                    Message = "Comunicado no encontrado"
                };
            }

            communicateEntity.Tittle = model.Tittle;
            communicateEntity.Content = model.Content;
            communicateEntity.Date = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();

            var communicateDto = _mapper.Map<CommunicateDto>(communicateEntity);

            return new ResponseDto<CommunicateDto>
            {
                Status = true,
                StatusCode = 200,
                Message = "Comunicado actualizado correctamente",
                Data = communicateDto
            };
        }
        //eliminar comunicado por id
        public async Task<ResponseDto<CommunicateDto>> DeleteCommunicate(Guid id)
        {
            var communicateEntity = await _dbContext.Communicates.FirstOrDefaultAsync(x => x.Id == id);

            if (communicateEntity == null)
            {
                return new ResponseDto<CommunicateDto>
                {
                    Status = false,
                    StatusCode = 404,
                    Message = "Comunicado no encontrado"
                };
            }

            _dbContext.Communicates.Remove(communicateEntity);
            await _dbContext.SaveChangesAsync();

            var communicateDto = _mapper.Map<CommunicateDto>(communicateEntity);

            return new ResponseDto<CommunicateDto>
            {
                Status = true,
                StatusCode = 200,
                Message = "Comunicado eliminado correctamente",
                Data = communicateDto
            };
        }
    }
}