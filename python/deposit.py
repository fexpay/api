# -*- coding:utf-8 -*-
import binascii
import hashlib
import json
import urllib
from pyDes import des, CBC, PAD_PKCS5
from collections import OrderedDict

key = '83A6AEDE-3B5A-4D3E-B789-DC780421C1A1'
smac = 'C628C57D3F0FCB7652D9C5D64898DFFF'

def des_encrypt(s):
    """
    DES 加密
    :param s: 原始字符串
    :return: 加密后字符串，16进制
    """
    iv = key
    k = des(key, CBC, iv, pad=None, padmode=PAD_PKCS5)
    en = k.encrypt(s, padmode=PAD_PKCS5)
    return binascii.b2a_hex(en)


def des_descrypt(s):
    """
    DES 解密
    :param s: 加密后的字符串，16进制
    :return:  解密后的字符串
    """
    iv = key
    k = des(key, CBC, iv, pad=None, padmode=PAD_PKCS5)
    de = k.decrypt(binascii.a2b_hex(s), padmode=PAD_PKCS5)
    return de

# 创建md5对象
hl = hashlib.md5()
hl.update(key.encode(encoding='utf-8'))
key = hl.hexdigest()
key = key[0:8].upper()

print("key:" + key)

# 定义入金数据
data = OrderedDict()
data["realName"] = urllib.quote("实名02")
data["amount"] = "1000"
data["orderId"] = "2018071318275278924"
data["idCard"] = "2222222222"
data["notifyUrl"] = "https://www.domain.com/notify/callback"
data["returnUrl"] = "https://www.domain.com/return/callback"
data["sendTime"] = "2018-07-13 18:27:52"

# 定义加密数据
# 签名入金数据
hl2 = hashlib.md5();
encodedData = json.dumps(data, separators=(',',':')) + smac

hl2.update(encodedData.encode(encoding='utf-8'))
sign=hl2.hexdigest()

ciphertext = OrderedDict()
ciphertext["data"] = data
ciphertext['sign'] = sign.upper()

#加密入金数据
str_en = des_encrypt(json.dumps(ciphertext, separators=(',',':'))).upper()
print(str_en)

#解密入金数据
str_de = des_descrypt(str_en)
print(str_de)
