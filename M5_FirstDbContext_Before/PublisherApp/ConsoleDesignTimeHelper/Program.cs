using ContractBC.ValueObjects;
using System.Text.Json.Serialization;
using System.Text.Json;
using ContractBC.ContractAggregate;
using System.Linq;

var spec = new SpecificationSet(1,1,1,1,1,true,true,1,10);
string fileName = "SpecificationSet.json";
string jsonString = JsonSerializer.Serialize(spec);
File.WriteAllText(fileName, jsonString);

//var a1 = new Author("julie", "lerman");
//var a2 = new Author("rich", "flynn");
//var authors = new List<Author> { a1, a2 };
//    var inits= new string(authors.SelectMany(a => a.Name.Initials).ToArray());
//var initfromchar = new string(inits);

