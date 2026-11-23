using System.Linq.Expressions;
using MediatR;
using MedicalAppointments.Notifications.Application.DTOs;
using MedicalAppointments.Notifications.Domain.Entities;
using MedicalAppointments.Notifications.Domain.Interfaces;
using Nuget.Persistence.Common;

namespace MedicalAppointments.Notifications.Application.Queries.GetNotificacionesPaged;

public sealed class GetNotificacionesPagedQueryHandler
    : IRequestHandler<GetNotificacionesPagedQuery, HttpResponse<PagedNotificacionDto>>
{
    private readonly INotificacionRepository _repository;

    public GetNotificacionesPagedQueryHandler(INotificacionRepository repository)
    {
        _repository = repository;
    }

    public async Task<HttpResponse<PagedNotificacionDto>> Handle(
        GetNotificacionesPagedQuery request,
        CancellationToken cancellationToken)
    {
        if (request.PageNumber < 1 || request.PageSize < 1)
        {
            return new HttpResponse<PagedNotificacionDto>
            {
                Success = false,
                Message = "pageNumber y pageSize deben ser mayores o iguales a 1."
            };
        }

        var resultado = await _repository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            BuildFilter(request),
            orderBy: null,
            orderByExpression: q => q.OrderByDescending(n => n.FechaRegistro),
            asNoTracking: true,
            splitQuery: false,
            cancellationToken: cancellationToken);

        return new HttpResponse<PagedNotificacionDto>
        {
            Success = true,
            Message = "Consulta paginada de notificaciones exitosa.",
            Data = new PagedNotificacionDto
            {
                TotalRecords = resultado.TotalRecords,
                TotalPage = resultado.TotalPage,
                CurrentPage = resultado.CurrentPage,
                PageSize = resultado.PageSize,
                Data = resultado.Data.Select(n => n.ToDto()).ToList()
            }
        };
    }

    private static Expression<Func<Notificacion, bool>>? BuildFilter(GetNotificacionesPagedQuery request)
    {
        Expression<Func<Notificacion, bool>>? filter = null;

        if (!string.IsNullOrWhiteSpace(request.Estado))
        {
            var estado = request.Estado.Trim().ToLower();
            filter = Combine(filter, n => n.Estado != null && n.Estado.ToLower() == estado);
        }

        if (!string.IsNullOrWhiteSpace(request.Canal))
        {
            var canal = request.Canal.Trim().ToLower();
            filter = Combine(filter, n => n.Canal != null && n.Canal.ToLower() == canal);
        }

        var tipo = request.Tipo ?? request.TipoNotificacion;
        if (tipo.HasValue)
        {
            var tipoNotificacion = tipo.Value;
            filter = Combine(filter, n => n.TipoNotificacion == tipoNotificacion);
        }

        if (request.FechaDesde.HasValue)
        {
            var fechaDesde = request.FechaDesde.Value;
            filter = Combine(filter, n => n.FechaRegistro >= fechaDesde);
        }

        if (request.FechaHasta.HasValue)
        {
            var fechaHasta = request.FechaHasta.Value;
            filter = Combine(filter, n => n.FechaRegistro <= fechaHasta);
        }

        return filter;
    }

    private static Expression<Func<Notificacion, bool>> Combine(
        Expression<Func<Notificacion, bool>>? actual,
        Expression<Func<Notificacion, bool>> adicional)
    {
        if (actual is null)
            return adicional;

        var parametro = actual.Parameters[0];
        var cuerpo = new ParameterReplacer(adicional.Parameters[0], parametro).Visit(adicional.Body)!;

        return Expression.Lambda<Func<Notificacion, bool>>(
            Expression.AndAlso(actual.Body, cuerpo),
            parametro);
    }

    private sealed class ParameterReplacer : ExpressionVisitor
    {
        private readonly ParameterExpression _origen;
        private readonly ParameterExpression _destino;

        public ParameterReplacer(ParameterExpression origen, ParameterExpression destino)
        {
            _origen = origen;
            _destino = destino;
        }

        protected override Expression VisitParameter(ParameterExpression node)
            => ReferenceEquals(node, _origen) ? _destino : base.VisitParameter(node);
    }
}
