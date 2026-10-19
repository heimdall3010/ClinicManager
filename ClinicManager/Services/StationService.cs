using System.Collections.Generic;
using System.Linq;
using ClinicManager.Data;
using ClinicManager.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicManager.Services;

public class StationService
{
    public List<Station> GetAllStations()
    {
        using AppDbContext context = new AppDbContext();

        return context.Stations
            .AsNoTracking()
            .OrderBy(station => station.StationId)
            .ToList();
    }

    public Station? GetStationByStationId(int stationId)
    {
        using AppDbContext context = new AppDbContext();

        return context.Stations
            .AsNoTracking()
            .FirstOrDefault(station => station.StationId == stationId);
    }

    public Station AddStation(Station station)
    {
        using AppDbContext context = new AppDbContext();

        context.Stations.Add(station);
        context.SaveChanges();

        return station;
    }

    public Station UpdateStation(Station station)
    {
        using AppDbContext context = new AppDbContext();

        context.Stations.Update(station);
        context.SaveChanges();

        return station;
    }
}