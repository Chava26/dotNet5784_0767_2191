using System;
using System.Collections.Generic;
namespace BlApi;
public interface IVolunteer:IObservable
{
    /// <summary>
    /// Logs in a user by username and password.
    /// </summary>
    /// <param name="username">The username of the user.</param>
    /// <param name="password">The password of the user.</param>
    /// <returns>The role of the user if login is successful.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown if the username or password is incorrect.</exception>
    BO.Role Login(string username, string password);

    /// <summary>
    /// Requests a filtered and sorted list of volunteers.
    /// </summary>
    /// <param name="isActive">Nullable boolean to filter active or inactive volunteers. If null, returns all.</param>
    /// <param name="sortField">Optional enum to specify the sorting field. Defaults to sorting by ID if null.</param>
    /// <returns>A filtered and sorted list of volunteers.</returns>
    public IEnumerable<BO.VolunteerInList> GetVolunteersList(bool? isActive = null, BO.VolunteerSortField? sortBy = null);
    /// <summary>
    /// Retrieves the details of a specific volunteer.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer.</param>
    /// <returns>A Volunteer object containing the volunteer's details and their current call in progress.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if no volunteer with the specified ID exists.</exception>
    BO.Volunteer GetVolunteerDetails(int volunteerId);

    /// <summary>
    /// Updates the details of a specific volunteer.
    /// </summary>
    /// <param name="requesterId">The ID of the user requesting the update.</param>
    /// <param name="volunteer">A Volunteer object with updated details.</param>
    /// <exception cref="UnauthorizedAccessException">Thrown if the requester is not authorized to perform the update.</exception>
    /// <exception cref="ArgumentException">Thrown if the provided details are invalid.</exception>
    void UpdateVolunteer(int requesterId, BO.Volunteer volunteer);

    /// <summary>
    /// Deletes a specific volunteer by ID.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer to delete.</param>
    /// <exception cref="InvalidOperationException">Thrown if the volunteer is currently handling a call or has handled calls before.</exception>
    /// <exception cref="KeyNotFoundException">Thrown if no volunteer with the specified ID exists.</exception>
    void DeleteVolunteer(int volunteerId);

    /// <summary>
    /// Adds a new volunteer.
    /// </summary>
    /// <param name="volunteer">A Volunteer object containing the details of the new volunteer.</param>
    /// <exception cref="ArgumentException">Thrown if the provided details are invalid.</exception>
    /// <exception cref="InvalidOperationException">Thrown if a volunteer with the same ID already exists.</exception>
    void AddVolunteer(BO.Volunteer volunteer);
}
