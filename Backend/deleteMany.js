const { MongoClient } = require("mongodb")
const uri = require("./atlas_uri")

const client = new MongoClient(uri)
const dbname = "fullerene"
var userCollection;


async function run() {
  try {
    const database = client.db("fullerene");
    const movies = database.collection("users");
    /* Delete all documents that match the specified regular
    expression in the title field from the "movies" collection */
    const query = { email: { $regex: "-" } };
    const result = await movies.deleteMany(query);
    // Print the number of deleted documents
    console.log("Deleted " + result.deletedCount + " documents");
  } finally {
    // Close the connection after the operation completes
    await client.close();
  }
}
// Run the program and print any thrown exceptions
run().catch(console.dir);