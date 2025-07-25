const { MongoClient } = require("mongodb")
const uri = require("./atlas_uri")

const client = new MongoClient(uri)
const dbname = "fullerene"
var userCollection;

//connects to a database
const connectToDatabase = async () => {
    try {
        await client.connect();
        console.log(`Connected to ${dbname} database`);

    } catch (err) {
        console.error(`error ${err} occured`);
    }
};

app.listen(port, hostname, async () => {
    try {
        await client.connect();
        console.log(`Connected to cluster`);
        userCollection = client.db("fullerene").collection("users");
        console.log(`Connected to collection`);
        let result = await userCollection.find({}).toArray();
        console.log(result);
    } catch (err) {
        console.error(`error ${err} occured`);
    }

    userCollection.
    //sendEMail(baseMailOptions);
});
