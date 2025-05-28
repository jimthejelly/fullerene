
//MongoDB
const { MongoClient } = require("mongodb")
const uri = require("./atlas_uri")

const client = new MongoClient(uri)
const dbname = "fullerene"


const connectToDatabase = async () => {
    try {
        await client.connect();
        console.log(`Connected to ${dbname} database`);

    } catch (err) {
        console.error(`error ${err} occured`);
    }
};



//Express/NodeJs
const express = require('express');
const bodyParser = require('body-parser');
const app = express();
const port = 3000;
const hostname = '127.0.0.1';



app.use(bodyParser.json());

app.get('/fullerene', async (req, res) => {
    console.log('Got pinged!');
    try {
        let result = await userCollection.find({}).toArray();
        console.log(result);
        res.send(result);

    }
    catch (err) {
        console.error(`error ${err} occured`);
    }
});

app.get('/fullerene/:user', async (req, res) => {
    console.log(`Got pinged for ${req.params.user}!`);
    try {
        let result = await userCollection.findOne({ "username": req.params.user });
        console.log(result);
        res.send(result);

    }
    catch (err) {
        console.error(`error ${err} occured`);
    }
});

var userCollection;


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


    console.log(`Server listening on port ${port}`);
});