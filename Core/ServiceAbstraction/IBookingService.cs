using Shared.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstraction
{
    public interface IBookingService
    {
        
      
        Task<IEnumerable<BookingDto>> GetAllBookingsAsync();
        Task<BookingDto?> GetBookingByIdAsync(int id);
        Task AddBookingAsync(BookingDto bookingDto);
        Task AddBookingWithPatientAsync(CreateBookingDto createBookingDto);
        Task UpdateBookingStatusAsync(int id, string status);
        Task<IEnumerable<BookingDto>> GetAllBookingPatientByIdAsync(int id);
        Task<bool> CancelBookingByPatientAsync(int bookingId, string phone);
        Task<bool> UpdateBookingAndPatientByPatientAsync(int bookingId, string phone, UpdateBookingAndPatientDto dto);
        Task<IEnumerable<BookingDto>> SearchBookingsAsync(string search);
        Task<int> CountBookingsAsync();
        Task<int> CountBookingsByStatusAsync(string status);
        Task<int> CountBookingsTodayAsync();
        Task<int> CountBookingsThisWeekAsync();
        Task<DateTime?> GetLastBookingDateAsync();
        Task<IEnumerable<BookingDto>> GetTodayBookingsAsync();








    }
}

