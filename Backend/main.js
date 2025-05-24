const { MongoClient } = require("mongodb");
const Express = require("express");
const BodyParser = require('body-parser');

const server = Express();
const client = new MongoClient(process.env["ATLAS_URI"]);

server.use(BodyParser.json());
server.use(BodyParser.urlencoded({ extended: true }));

var collection;

server.post("/fullerene", async (request, response, next) => {
    try {
        let result = await collection.insertOne(request.body);
        response.send(result);
    } catch (e) {
        response.status(500).send({ message: e.message });
    }
});

server.get("/fullerene", async (request, response, next) => {
    try {
      let result = await collection.find({}).toArray();
      response.send(result);
    } catch (e) {
        response.status(500).send({ message: e.message });
    }
});

server.get("/fullerene/:username", async (request, response, next) => {
    console.debug("part 2 baby");
    try {
        let result = await collection.findOne({ "username": request.params.username });
        response.send(result);
    } catch (e) {
        response.status(500).send({ message: e.message });
    }
});

server.put("/fullerene/:username", async (request, response, next) => {
    try {
        let result = await collection.updateOne(
            { "username": request.params.username },
            { "$set": request.body }
        );
        response.send(result);
    } catch (e) {
        response.status(500).send({ message: e.message });
    }
});

server.delete("/fullerene/:username", async (request, response, next) => {
    try {
        let result = await collection.deleteOne({ "username": request.params.username });
        response.send(result);
    } catch (e) {
        response.status(500).send({ message: e.message });
    }
});

server.listen("3000", async () => {
    try {
        await client.connect();
        collection = client.db("fullerene").collection("fullerene");
        console.log("Listening at :3000...");
    } catch (e) {
        console.error(e);
    }
});