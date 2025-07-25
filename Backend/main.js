
//MongoDB requirements
const { MongoClient } = require("mongodb")
const uri = require("./atlas_uri")//hidden passkey so that people cant simply access the database

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


//Needed to determine IP of whatever is being run on
var os = require('os');
var networkInterfaces = os.networkInterfaces();
//console.log(networkInterfaces.eth0[0].address);

//Express/NodeJs requirements
const express = require('express');
const bodyParser = require('body-parser');
const app = express();
const port = 3000;//
const hostname = networkInterfaces.eth0[0].address;//will have to be changes to something else when not just on my machine


//nodemailer req's 
var nodemailer = require('nodemailer');
const passcode = require("./gmailPasscode");//"hidden" passcode so that people cant just copy paste things

var transporter = nodemailer.createTransport({
  service: 'gmail',
  auth: {
    user: 'FullereneRPI@gmail.com',
    pass: passcode
  }
});

//Ex of what an email JSON should look like
var baseMailOptions = {
  from: 'FullereneRPI@gmail.com',
  to: 'brandjsebastien@gmail.com',
  subject: 'Sending Email using Node.js',
  text: 'That was easy!'
};



app.use(bodyParser.json());

app.get('/:t',async (req,res)=>{
    console.log("generic");
    res.send("generic");  
});

//get all users? shouldnt ever really be used, could think of contexts, but we'll cross that bridge later
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

//get a user defined by the user tag - Login
app.get('/fullerene/:user', async (req, res) => {
    console.log(`Got pinged for ${req.params.user}!`);
    try {
        //gets the current time
        var currentDate=new Date().toISOString();    
        //searches 
        let result = await userCollection.findOneAndUpdate({ "username": req.params.user },{$inc: {numberOfLogins:1},$set:{lastLogin: currentDate}});
        //TODO: Update login setTimeout(function() {}, 10);
        console.log(result);
        res.send(result);
    }
    catch (err) {
        console.error(`error ${err} occured`);
    }
});

//validate user
app.put("/fullerene", async (req, res) => {
    try {
        console.log("verifying");
        //console.log(req.body);    
        result=null;
        if(req.body.code!=null){
            //we are validating
            let code=req.body.code;
            let user=req.body.username;
            let found = await find(user);
            
            if (found==null){
                
                //they are attempting to validate a nonexistant user, yikes
                result = {
                    success: false,
                    error: true,
                    reason: "invalid user"
                };
            }
            else if ( found.verificationCode==null){
                
                //attemptong to verify already verified account
                result = {
                    success: false,
                    error: true,
                    reason: "already verified"
                };
            }
            else{                
                
                //verify them
                if(found.verificationCode==code){
                    
                    //we good
                    result = await userCollection.updateOne({"username":user},{$unset: {verificationCode:-1}},{$set:{numberOfLogins:0}});
                }
                else{
                    //bad code
                    result = {
                    success: false,
                    error: false,
                    reason: "invalid verification code"
                };
                }
                
            }
        }
        console.log(result);
        res.send(result);
    } catch (e) {
        res.status(500).send({ message: e.message });
    }
});



//Server side ensure uniqueness in users. storing in server feels bad
// make a call to db requesting username. If username exists, result false

async function find(username){
    if(username==null || username==""){ return null;}
    try{
        let search = await userCollection.findOne({ "username": username });
        console.log("searched");
        console.log(search);
        return search;
    }
    catch (err){
        console.error(`searcherror ${err} occured`);
        return null;
    }
}

function parse(json){
    let str= JSON.stringify(json);
    let ind= str.search(':');
    str= str.substring(ind+2);
    ind = str.search('\"');
    return str.substring(0,ind);
}

//add a User
app.post("/fullerene", async (req, res) => {
    
    try {

        console.log("asked to create life");

        //this is a JSON with all of the info sent by the user
        //Note - if searching for a category that doesnt exist in the JSON, its null        
        let input = req.body;


        //looks for if that username already in the database
        let found =  await find(input.username);

        //thing we'll be returning
        let result= null;

        if (found == null){
            console.log("found did not return  an item, add");


            let mail=input.email;
            //TODO: replace if with more genereal 'bad email input'
            if(mail==null||mail==""){
                console.log("email problem: "+mail+"<<");
                result = {
                    success: false,
                    error: true,
                    reason: "invalid email"
                };
                res.send(result);
                
                return;
                
            }

            let code= Math.floor(Math.random()*1000000 );
            console.log(code);
            input.verificationCode=code;
            var currentDate=new Date().toISOString();    
            input.firstLogin=currentDate;
            console.log(currentDate);
            result = await userCollection.insertOne(input);

            var regOptions = {
              from: 'FullereneRPI@gmail.com',
              to: mail,
              subject: 'Account Verification',
              text: 'Enter Code: '+code+' to verify account'
            };
            sendEMail(regOptions)
        }

        else{
            console.log("found returned an item, dont add");
            
            result = {
                success: false,
                error: false,
                reason: "username already taken"
            };
            //errors out at this line - dont know how - need to find some way of passing this fucking info back.
            
        }
        
        //let reesult=true;

        console.log(">>>>");
        console.log(result);
        console.log("<<<<");
        res.send(result);
    } catch (e) {
        console.error(`searcherror ${e} occured`);
        res.status(500).send({ message: e.message });
    }
});






//effectively 'main', starts a connection with MongoDB, and also starts listening to localhost/3000
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

//    console.log(networkInterfaces);
    console.log(`Server listening on port ${port} on IP ${hostname}`);
    //sendEMail(baseMailOptions);
});




//is given an email JSON and sends it, 
//all emails should be sent from fullereneRPI until further notice
function sendEMail(mailOp){
    transporter.sendMail(mailOp, function(error, info){
      if (error) { console.log(error); } 
      else { console.log('Email sent: ' + info.response); }
    } );    
}
