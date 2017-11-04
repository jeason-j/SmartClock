﻿import net = require("net");

interface configValue {
    set: number,
    get:number,
    value: Buffer
}


var configValues: configValue[] = [
    { set: 0x10, get: 0x11, value: new Buffer([0x00, 0x02]) },
    { set: 0x1E, get: 0x1C, value: new Buffer([0x00]) },
    { set: 0x1F, get: 0x1D, value: new Buffer([0x00]) },
    { set: 0x0D, get: 0x0C, value: new Buffer([0x00]) },
    { set: 0x07, get: 0x06, value: new Buffer([0x00]) },
];

var server = net.createServer((s: net.Socket) => {
    s.on("data", (data: Buffer) => {
        var d = new Date().toLocaleTimeString();
        console.log(d,"received", data);
        if (data[0] != 0xA5) {
            return; //ignore none E-Ink commands
        }
        for (var i = 0; i < configValues.length; i++) {
            let tmp: Buffer = configValues[i].value;

            if (configValues[i].set == data[3]) {
                data.copy(tmp, 0, 4, tmp.length + 4);//store config data
                
                break;
            }

            if (configValues[i].get == data[3]) {
                s.write(tmp);
                console.log("send", tmp);
                return;
            }
        }
        s.write("OK");
        console.log("send OK");
    });
});
server.listen(2121, () => { console.log("Server Started at port ", server.address()); });
