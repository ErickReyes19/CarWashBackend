using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

public class OrderHub : Hub
{
    // Método para que un empleado se una al grupo correspondiente
    public async Task JoinRoom(string employeeId)
    {
        var groupName = $"employee-{employeeId}"; // Grupo basado en el employeeId
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName); // Agregar al grupo
        Console.WriteLine($"Empleado {employeeId} se ha unido al grupo {groupName}"); // Log para verificar
    }

    // Método para que un empleado salga del grupo
    public async Task LeaveRoom(string employeeId)
    {
        var groupName = $"employee-{employeeId}"; // Grupo basado en el employeeId
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName); // Eliminar del grupo
        Console.WriteLine($"Empleado {employeeId} ha salido del grupo {groupName}"); // Log para verificar
    }

    // Método para enviar la orden a un grupo (empleado específico)
    public async Task SendOrderToEmployee(string employeeId, Order order)
    {
        var groupName = $"employee-{employeeId}"; // El grupo es el que se creó con el employeeId
        await Clients.Group(groupName).SendAsync("newOrder", order); // Enviar el mensaje al grupo
    }
}
