using System;
using System.Linq;
using eBookLibrary.Models;

public class NotificationService
{
    private readonly ApplicationDbContext _context;

    public NotificationService(ApplicationDbContext context)
    {
        _context = context;
    }

    public void SendReminderNotifications()
    {
        var reminderDate = DateTime.Now.AddDays(5);

        var booksToRemind = _context.BorrowedBooks
            .Where(b => b.ReturnDate.Date == reminderDate.Date)
            .ToList();

        foreach (var book in booksToRemind)
        {
            // Implement your notification logic here
            Console.WriteLine($"Reminder: User {book.UserId}, please return '{book.BookId}' by {book.ReturnDate}.");
        }
    }
}
