using Microsoft.AspNetCore.Mvc;
using Npgsql;
using MagellanTest.Models;

namespace MagellanTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ItemsController : ControllerBase
    {
        /// <summary>
        /// This is where one can reset the user id and password of the database
        /// or one can change the setting in the appsettings.json.
        /// </summary>
        NpgsqlConnection _conn = new NpgsqlConnection("Server=localhost; User Id=postgres; " + 
            "Password=password"
        );

        /// <summary>
        /// Create a new record in the item table.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost()]
        public IActionResult CreateItem(ItemRequest item)
        {
            try
            {
                _conn.Open();
                NpgsqlCommand command = 
                new NpgsqlCommand("INSERT INTO part.item (item_name, parent_item, cost, req_date) " + 
                    "values (@item_name, @parent_item, @cost, @date) " + 
                    "RETURNING id;", _conn);
                command.Parameters.Add("@item_name", NpgsqlTypes.NpgsqlDbType.Varchar).Value = item.ItemName;
                command.Parameters.Add("@parent_item", NpgsqlTypes.NpgsqlDbType.Integer).Value =
                 Convert.IsDBNull(item.ParentItem) ? item.ParentItem : DBNull.Value;
                command.Parameters.Add("@cost",NpgsqlTypes.NpgsqlDbType.Integer).Value = item.Cost;
                command.Parameters.Add("@date",NpgsqlTypes.NpgsqlDbType.Date).Value = item.ReqDate;
                var dr = command.ExecuteReader();
            
                while(dr.Read())
                {
                    return Ok(dr[0]);
                }

                return Ok("Id is not present");
            } catch(Exception ex)
            {
                return StatusCode(500);
            } finally
            {
                 _conn?.Close();
            }

        }

        /// <summary>
        /// Query the item table by supplying the id of the record.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult GetItem(int id)
        {
            try
            {
                _conn.Open();
                NpgsqlCommand command = new NpgsqlCommand("SELECT * from part.item where id=@id", _conn);
                command.Parameters.Add("@id", NpgsqlTypes.NpgsqlDbType.Integer).Value=id;
                using (var dr = command.ExecuteReader())
                {
                    while(dr.Read() && dr.HasRows)
                    {
                        var result = new Item
                        {
                            Id = (int)dr["id"],
                            ItemName = dr["item_name"].ToString(),
                            ParentItem = !Convert.IsDBNull(dr["parent_item"])? (int?) dr["parent_item"] :  null ,
                            Cost = (int)dr["cost"],
                            ReqDate = DateTime.Parse(dr["req_date"].ToString())
                        };
                        _conn.Close();
                        return Ok(result);
                    }
                }
                return NotFound("No result found");
            }catch(Exception ex)
            {
                return StatusCode(500);
            } finally 
            {
                _conn?.Close();
            }
        }

        /// <summary>
        /// An endpoint that calls the Get_Total_Cost function.
        /// </summary>
        /// <param name="item_name"></param>
        /// <returns></returns>
        [HttpGet("/cost/{item_name}")]
        public IActionResult Get_Total_Cost(string item_name)
        {
            try 
            {
                _conn.Open();
                NpgsqlCommand command = new NpgsqlCommand("select * from part.Get_Total_Cost(@item_name);", _conn);
                command.Parameters.Add("@item_name", NpgsqlTypes.NpgsqlDbType.Varchar).Value = item_name;
                int? sum = !Convert.IsDBNull(command.ExecuteScalar()) ?
                    (int)(long)command.ExecuteScalar() : null;

                _conn.Close();
                return Ok(sum);
            }
            catch(Exception ex)
            {
                return Ok("Parent_item not unique");
                
            } finally{
                _conn?.Close();
            }
        }
    }
}
