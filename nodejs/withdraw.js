'use strict';

const crypto = require('crypto');

let key = '83A6AEDE-3B5A-4D3E-B789-DC780421C1A1';
// smac
let smac = 'C628C57D3F0FCB7652D9C5D64898DFFF';

var data = {
	'realName' : encodeURI('实名02'),
    'coinId' : '1',
	'amount' : '0.48',
	'orderId' : '2018071318275278925',
	'idCard' : '2222222222',
	'notifyUrl' : 'https://www.domain.com/notify/withdraw/callback',
	'returnUrl' : 'https://www.domain.com/return/withdraw/callback',
	'sendTime' : '2018-07-13 18:27:52'
};

key = crypto.createHash('md5').update(key, 'utf-8').digest('hex').substr(0, 8).toUpperCase();

var ciphertext = {
	'data' : data,
	'sign' : crypto.createHash('md5').update(JSON.stringify(data) + smac, 'utf-8').digest('hex').toUpperCase()
};

let cipherStr = JSON.stringify(ciphertext);

let iv = key;

// encrypt data
let cipher = crypto.createCipheriv('des-cbc', key, iv);
let encryptedData = cipher.update(cipherStr, 'utf8', 'hex') + cipher.final('hex');
encryptedData = encryptedData.toUpperCase();

console.log(encryptedData);

var decipher = crypto.createDecipheriv('des-cbc', key, iv);
var decoded = decipher.update(encryptedData, 'hex', 'utf8');
decoded += decipher.final('utf8');

console.log('\n' + decoded);
