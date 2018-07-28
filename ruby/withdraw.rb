OKEY = "83A6AEDE-3B5A-4D3E-B789-DC780421C1A1"
SMAC = "C628C57D3F0FCB7652D9C5D64898DFFF"

require 'openssl'

class Des
	
	require 'base64'
	ALG = 'DES-CBC'

	def encode(key, iv, str)
		des = OpenSSL::Cipher.new(ALG)
		des.key = key
		des.iv = iv
		des.encrypt

		cipher = des.update(str)
		cipher << des.final
		str = ""
    	cipher.each_byte {|c| str += ("%02x" % c);}
    	return str.upcase
	end

	def decode(key, iv, str)
		str = [str].pack('H*')
		des = OpenSSL::Cipher.new(ALG)
		des.key = key
		des.iv = iv
		des.decrypt
		des.update(str) + des.final
	end
end


require 'pp'
require 'digest'
require 'json'
require 'uri'

md5 = Digest::MD5.new
md5.update(OKEY)
KEY = md5.hexdigest[0,8].upcase
IV = KEY

des = Des.new

data = {
    'realName' => URI::encode('实名02'),
    'coinId' => '1',
    'amount' => '0.48',
    'orderId' => '2018071318275278925',
    'idCard' => '2222222222',
    'notifyUrl' => 'https://www.domain.com/notify/withdraw/callback',
    'returnUrl' => 'https://www.domain.com/return/withdraw/callback',
    'sendTime' => '2018-07-13 18:27:52'
}

md5_sign = Digest::MD5.new
md5_sign.update(data.to_json() + SMAC)
SIGN = md5_sign.hexdigest

ciphertext = {
	"data" => data,
	"sign" => SIGN.upcase
}

pp '=======encrypt========'
str = des.encode(KEY, IV, ciphertext.to_json())
pp str

pp '=======decrypt========'

pp des.decode(KEY, IV, str)
