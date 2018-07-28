#include <stdio.h>
#include <stdlib.h>
#include <string>
#include <algorithm>  
#include "Md5.hpp"
#include <map>
#include <iostream>
#include <openssl/objects.h>
#include <openssl/evp.h>
#include <openssl/des.h>

using namespace std;

void readJson();
void writeJson();

string skey = "83A6AEDE-3B5A-4D3E-B789-DC780421C1A1";
string smac = "C628C57D3F0FCB7652D9C5D64898DFFF";

int hexCharToInt(char c)  
{   
    if (c >= '0' && c <= '9') return (c - '0');  
    if (c >= 'A' && c <= 'F') return (c - 'A' + 10);  
    if (c >= 'a' && c <= 'f') return (c - 'a' + 10);  
    return 0;  
}  
  
char* hexstringToBytes(string s)  
{           
    int sz = s.length();  
    char *ret = new char[sz/2];  
    for (int i=0 ; i <sz ; i+=2) {  
        ret[i/2] = (char) ((hexCharToInt(s.at(i)) << 4)  
                        | hexCharToInt(s.at(i+1)));  
    }  
    return ret;  
}  
  
string bytestohexstring(char* bytes,int bytelength)  
{  
  string str("");  
  string str2("0123456789abcdef");   
   for (int i=0;i<bytelength;i++) {  
     int b;  
     b = 0x0f&(bytes[i]>>4);  
     str.append(1,str2.at(b));            
     b = 0x0f & bytes[i];  
     str.append(1,str2.at(b));  
   }  
   return str;  
}

char *bin2hex(unsigned char *data, int size)
{
    int  i = 0;
    int  v = 0;
    char *p = NULL;
    char *buf = NULL;
    char base_char = 'A';
 
    buf = p = (char *)malloc(size * 2 + 1);
    for (i = 0; i < size; i++) {
        v = data[i] >> 4;
        *p++ = v < 10 ? v + '0' : v - 10 + base_char;
 
        v = data[i] & 0x0f;
        *p++ = v < 10 ? v + '0' : v - 10 + base_char;
    }
 
    *p = '\0';
    return buf;
}

unsigned char *hex2bin(const char *data, int size, int *outlen)
{
    int i = 0;
    int len = 0;
    char char1 = '\0';
    char char2 = '\0';
    unsigned char value = 0;
    unsigned char *out = NULL;
 
    if (size % 2 != 0) {
        return NULL;
    }
 
    len = size / 2;
    out = (unsigned char *)malloc(len * sizeof(char) + 1);
    if (out == NULL) {
        return NULL;
    }
 
    while (i < len) {
        char1 = *data;
        if (char1 >= '0' && char1 <= '9') {
            value = (char1 - '0') << 4;
        }
        else if (char1 >= 'a' && char1 <= 'f') {
            value = (char1 - 'a' + 10) << 4;
        }
        else if (char1 >= 'A' && char1 <= 'F') {
            value = (char1 - 'A' + 10) << 4;
        }
        else {
            free(out);
            return NULL;
        }
        data++;
 
        char2 = *data;
        if (char2 >= '0' && char2 <= '9') {
            value |= char2 - '0';
        }
        else if (char2 >= 'a' && char2 <= 'f') {
            value |= char2 - 'a' + 10;
        }
        else if (char2 >= 'A' && char2 <= 'F') {
            value |= char2 - 'A' + 10;
        }
        else {
            free(out);
            return NULL;
        }
 
        data++;
        *(out + i++) = value;
    }
    *(out + i) = '\0';
 
    if (outlen != NULL) {
        *outlen = i;
    }
 
    return out;
}

string generateKeys()
{
    MD5_CTX md5;
    unsigned char key[8];

    MD5Init(&md5);
    MD5Update(&md5, (unsigned char *)skey.c_str(), skey.length());
    MD5Final(&md5, key);
    
    return bytestohexstring((char *)key, 4);
}

string signData(string dataStr)
{
    MD5_CTX md5;
    unsigned char buf[32];

    MD5Init(&md5);
    MD5Update(&md5, (unsigned char *)dataStr.c_str(), dataStr.length());
    MD5Final(&md5, buf);
    
    return bytestohexstring((char *)buf, 16);
}

string getString(const int32_t& a)
{
    char c[1024]={0};
    snprintf(c,sizeof(c),"%d",a);
    return c;
}

string getString(const int64_t& a)
{
    char c[1024]={0};
    snprintf(c,sizeof(c),"%lld",a);
    return c;
}

string getString(const string& s)
{
    return s;
}

std::string UrlEncode(const std::string& szToEncode)
{
	std::string src = szToEncode;
	char hex[] = "0123456789ABCDEF";
	string dst;
 
	for (size_t i = 0; i < src.size(); ++i)
	{
		unsigned char cc = src[i];
		if ( (cc >= 'A' && cc <= 'Z') 
                 || (cc >='a' && cc <= 'z')
                 || (cc >='0' && cc <= '9')
                 || cc == '.'
                 || cc == '_'
                 || cc == '-'
                 || cc == '*')
		{
			if (cc == ' ')
			{
				dst += "+";
			}
			else
				dst += cc;
		}
		else
		{
			unsigned char c = static_cast<unsigned char>(src[i]);
			dst += '%';
			dst += hex[c / 16];
			dst += hex[c % 16];
		}
	}
	return dst;
}

template<typename TypeOne,typename TypeTwo>
string mapToString(std::map<TypeOne,TypeTwo>& m)
{
    string str="{";
    typename std::map<TypeOne,TypeTwo>::iterator it = m.begin();
    int index = 0;
    for(;it != m.end();it++)
    {
        str += "\"";
        str += getString(it->first) + "\":\"" + getString(it->second);
        str += "\"";
        if ((index + 1) != m.size())
            str += ",";
        index ++;
    }
    str += "}";
    return str;
}

int encryptDes( unsigned char *key, unsigned char *iv,
    unsigned char *plain, unsigned char *cipher, int plen )
{
    int clen = 0;
    int tmplen = 0;
    EVP_CIPHER_CTX ctx;

    /* 01：加密初始化
     * 使用DES_CBC算法加密 */
    EVP_EncryptInit( &ctx, EVP_des_cbc(), key, iv );
    /* 设置不填充（缺省填充） */
    if( 0 == plen % 8 ){
        EVP_CIPHER_CTX_set_padding( &ctx, 0 );
    }

    /* 02：加密 */
    if( !EVP_EncryptUpdate( &ctx, cipher, &clen, plain, plen ) ){
        
        fprintf( stderr, "EVP_EncryptUpdate error\n" );
        return EXIT_FAILURE;
    }

    /* 03：加密后处理 */
    if( !EVP_EncryptFinal( &ctx, cipher + clen, &tmplen ) ){
        
        fprintf( stderr, "EVP_EncryptFinal error\n" );
        return EXIT_FAILURE;
    }
    clen += tmplen;

    return clen;
}

//入金数据加密示例
void deposit(string key, string smac) {
    cout << "\n==============入金示例==============\n";
    //构造入金数据
    string realName = UrlEncode("实名02");
    string data = "{";
    data += "\"realName\":\""+realName+"\",";
    data += "\"amount\":\"1000\",";
    data += "\"orderId\":\"2018071318275278924\",";
    data += "\"idCard\":\"2222222222\",";
    data += "\"notifyUrl\":\"https://www.domain.com/notify/callback\",";
    data += "\"returnUrl\":\"https://www.domain.com/return/callback\",";
    data += "\"sendTime\":\"2018-07-13 18:27:52\"";
    data += "}";

    //签名数据
    string sign = signData(data+smac);

    transform(sign.begin(), sign.end(), sign.begin(), ::toupper); 

    //构造入金加密数据
    string ciphertext = "{\"data\":" + data + ",\"sign\":\"" + sign + "\"}"; 

    //入金加密数据
    unsigned char cipher[2048 + 1] = {0};
    int clen = encryptDes((unsigned char *)(key.c_str()), (unsigned char *)key.c_str(), (unsigned char *)ciphertext.c_str(), cipher,ciphertext.length());
    
    string encrypted = bytestohexstring((char *)cipher, clen);
    transform(encrypted.begin(), encrypted.end(), encrypted.begin(), ::toupper);
    cout << encrypted << endl;
}

//出金验证加密示例
void checkWithdraw(string key, string smac) {
    cout << "\n==============出金验证示例==============\n";
    //构造入金数据
    string realName = UrlEncode("实名02");
    string data = "{";
    data += "\"realName\":\""+realName+"\",";
    data += "\"coinId\":\"1\",";
    data += "\"amount\":\"0.48\",";
    data += "\"idCard\":\"2222222222\"";
    data += "}";

    //签名数据
    string sign = signData(data+smac);

    transform(sign.begin(), sign.end(), sign.begin(), ::toupper); 

    //构造出金验证加密数据
    string ciphertext = "{\"data\":" + data + ",\"sign\":\"" + sign + "\"}"; 

    //cout << ciphertext << endl;

    //出金验证加密数据
    unsigned char cipher[2048 + 1] = {0};
    int clen = encryptDes((unsigned char *)(key.c_str()), (unsigned char *)key.c_str(), (unsigned char *)ciphertext.c_str(), cipher,ciphertext.length());
    
    string encrypted = bytestohexstring((char *)cipher, clen);
    transform(encrypted.begin(), encrypted.end(), encrypted.begin(), ::toupper);
    cout << encrypted << endl;
}

//出金数据加密示例
void withdraw(string key, string smac) {
    cout << "\n==============出金示例==============\n";
    //构造入金数据
    string realName = UrlEncode("实名02");
    string data = "{";
    data += "\"realName\":\""+realName+"\",";
    data += "\"coinId\":\"1\",";
    data += "\"amount\":\"0.48\",";
    data += "\"orderId\":\"2018071318275278925\",";
    data += "\"idCard\":\"2222222222\",";
    data += "\"notifyUrl\":\"https://www.domain.com/notify/withdraw/callback\",";
    data += "\"returnUrl\":\"https://www.domain.com/return/withdraw/callback\",";
    data += "\"sendTime\":\"2018-07-13 18:27:52\"";
    data += "}";

    //签名数据
    string sign = signData(data+smac);

    transform(sign.begin(), sign.end(), sign.begin(), ::toupper); 

    //构造出金加密数据
    string ciphertext = "{\"data\":" + data + ",\"sign\":\"" + sign + "\"}"; 

    //cout << ciphertext << endl;

    //出金加密数据
    unsigned char cipher[2048 + 1] = {0};
    int clen = encryptDes((unsigned char *)(key.c_str()), (unsigned char *)key.c_str(), (unsigned char *)ciphertext.c_str(), cipher,ciphertext.length());
    
    string encrypted = bytestohexstring((char *)cipher, clen);
    transform(encrypted.begin(), encrypted.end(), encrypted.begin(), ::toupper);
    cout << encrypted << endl;
}

int main(int argc, char** argv) {
    string key = generateKeys();

    transform(key.begin(), key.end(), key.begin(), ::toupper);  

    cout << key << endl;

    //入金加密示例
    deposit(key, smac);

    //出金验证加密示例
    checkWithdraw(key, smac);

    //出金加密示例
    withdraw(key, smac);

	return 0;
}
