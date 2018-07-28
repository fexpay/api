package main 

import (
	"bytes"
	"crypto/cipher"
	"crypto/md5"
	"crypto/des"
	"encoding/json"
	"encoding/hex"
	"net/url"
	"fmt"
	"strings"
	"github.com/axgle/mahonia"
)

//DES加密
func DesEncrypt(originData, key[]byte) (string, error) {
	var enc mahonia.Encoder
	
	enc = mahonia.NewEncoder("gbk")
	
	if enc == nil {
		
		return "", nil
	}
	originDataStr := enc.ConvertString(string(originData))
	originData = []byte(originDataStr)
	block, err := des.NewCipher(key)
	if err != nil {
		return "", err
	}
	
	originData = PKCS5Padding(originData, block.BlockSize())
	blockMode := cipher.NewCBCEncrypter(block, key)
	crypted := make([]byte, len(originData))
	blockMode.CryptBlocks(crypted, originData)
	encodeString := hex.EncodeToString(crypted)
	
	return encodeString, nil
}

func PKCS5Padding(ciphertext []byte, blockSize int) []byte {
	padding := blockSize - len(ciphertext) % blockSize
	padtext := bytes.Repeat([]byte{byte(padding)}, padding)
	return append(ciphertext, padtext...)
}

func DesDecrypt(encodeString string, key []byte)(string, error) {
	var dec mahonia.Decoder
	crypted, err := hex.DecodeString(encodeString)
	block, err := des.NewCipher(key)
	if err != nil {
		return "", err
	}

	blockMode := cipher.NewCBCDecrypter(block, key)
	originData := crypted
	blockMode.CryptBlocks(originData, crypted)
	originData = ZeroUnPadding(originData)
	dec = mahonia.NewDecoder("gbk")
	originDataStr := dec.ConvertString(string(originData))

	return originDataStr, nil
}

func ZeroUnPadding(originData []byte) []byte {
	length := len(originData)
	unpadding := int(originData[length - 1])

	return originData[:(length - unpadding)];
}

func Md5(s string) string {
	h := md5.New()
	h.Write([]byte(s))
	return hex.EncodeToString(h.Sum(nil))
} 

type DataOfParameters struct {
	RealName 	string   `json:"realName"`
	CoinId  	string   `json:"coinId"`
	Amount	 	string   `json:"amount"`
    OrderId     string   `json:"orderId"`
	IdCard   	string   `json:"idCard"`
    NotifyUrl   string   `json:"notifyUrl"`
    ReturnUrl   string   `json:"returnUrl"`
    SendTime    string   `json:"sendTime"`
}

type InterfaceParameters struct {
	Data DataOfParameters	`json:"data"`
	Sign string 			`json:"sign"`
}

func main() {
	key := "83A6AEDE-3B5A-4D3E-B789-DC780421C1A1"
	// smac
	smac := "C628C57D3F0FCB7652D9C5D64898DFFF"

	/**
 	 * 入金数据加密过程
 	*/
	// 接口参数次序如下
	data := DataOfParameters{}
	data.RealName = url.QueryEscape("实名02") 	// 客户真实姓名
	data.CoinId = "1"		                    // 1=BTC, 2=ETH, 3=UND
	data.Amount = "0.48"						// 出金指定币种对应币的数量
    data.OrderId = "2018071318275278925"        // 商家平台出金订单ID
	data.IdCard = "2222222222"					// 2222222222 客户证件号码
    data.NotifyUrl = "https://www.domain.com/notify/withdraw/callback" //出金成功后，立即调用Url
    data.ReturnUrl = "https://www.domain.com/return/withdraw/callback" //出金成功后，定时同步URL
    data.SendTime = "2018-07-13 18:27:52"       //出金时间

	marshalData, _ := json.Marshal(data)
	
	params := InterfaceParameters{}
	params.Data = data
	params.Sign = strings.ToUpper(Md5(string(marshalData) + smac))
	//s = string([]rune(s)[:3])
	key = strings.ToUpper(Md5(key))
	key = string([]byte(key)[:8])

    cipherStr, _ := json.Marshal(params)

    fmt.Println("入金数据加密")
    fmt.Println("原文:" + string(cipherStr))

	// DES CBC 加解密
    encryptedData, _ := DesEncrypt(cipherStr, []byte(key))

    fmt.Println("密文:" + strings.ToUpper(encryptedData))

    decryptedData, _ := DesDecrypt(encryptedData, []byte(key))
    fmt.Println("解密:" + decryptedData)
}
