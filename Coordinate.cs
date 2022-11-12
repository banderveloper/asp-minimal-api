using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.Diagnostics.Eventing.Reader;
using System.Reflection;

public record Coordinate(double Latitude, double Longitude)
{
    private static bool TryParse(string input, out Coordinate? coordinate)
    {
        coordinate = default;
        var splitArray = input.Split(',', 2);
        if (splitArray.Length != 2) return false;

        if (!double.TryParse(splitArray[0], out double lat)) return false;
        if (!double.TryParse(splitArray[1], out var lon)) return false;

        coordinate = new Coordinate(lat, lon);
        return true;
    }


    private static async ValueTask<Coordinate?> BindAsync(HttpContext context, ParameterInfo parameter)
    {
        return await Task.Run(() =>
        {
            var input = context.GetRouteValue(parameter.Name!) as string ?? string.Empty;
            TryParse(input, out var coordinate);
            return coordinate;
        });
    }
       
}