

using DO;

namespace DalApi;
/// <summary>
/// This interface defines basic CRUD (Create, Read, Update, Delete) operations 
/// for working with entities in the Data Access Layer (DAL).
/// </summary>

public interface ICrud<T> where T : class
    {
        public void Create(T item); //Creates new entity object in DAL
        T? Read(int id); //Reads entity object by its ID 
       IEnumerable<T> ReadAll(Func<T, bool>? filter = null); // Reads all entity objects
        void Update(T item); //Updates entity object
        void Delete(int id); //Deletes an object by its Id
        void DeleteAll(); //Delete all entity objects
    }


