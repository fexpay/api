package com.springcloud.config.demo;

import com.alibaba.fastjson.JSONObject;

public class TryDemo {
	private static String smac="C628C57D3F0FCB7652D9C5D64898DFFF";
	private static String key="83A6AEDE-3B5A-4D3E-B789-DC780421C1A1";
	
	public static void main(String[] args) throws Exception {
	
			//deposit();
			//checkWithdraw();
			withdraw();
			
			//decode();
	}
	
	// 入金接口数据加密方法
	public static void deposit() throws Exception {
		JSONObject json = new JSONObject();
		//转账数目
		json.put("amount", "80");
		//商家平台入金订单ID
		json.put("orderId", "2018071318275278924");
		//入金时间
		json.put("sendTime", "2017-08-30 22:05:31");
		//真实姓名
		json.put("realName", "实名02");
		//证件号码
		json.put("idCard", "2222222222");
		//入金成功后，定时 同步Url
		json.put("notifyUrl", "https://www.domain.com/notify/callback");
		//入金成功后，定时 同步Url
		json.put("returnUrl", "https://www.domain.com/return/callback");
		
		JSONObject jsonObject = new JSONObject();
		jsonObject.put("data", json);
		//签名
		jsonObject.put("sign", Md5.md5Digest(json.toJSONString() + smac));
		String ciphertext = DESUtil.ENCRYPTMethod(jsonObject.toJSONString(), key).toUpperCase();
		System.out.println(ciphertext);

		decode(ciphertext);
	}
	
	
	// 预出金接口数据加密方法
	public static void checkWithdraw() throws Exception {
		JSONObject json = new JSONObject();
		//真实姓名
		json.put("realName", "实名02");
		//证件号码
		json.put("idCard", "2222222222");
		//币种id：1=BTC, 2=ETH, 3=UND
		json.put("coinId", "1");
		//预出金指定币种对应币的数量
		json.put("amount", "0.48");
		
		JSONObject jsonObject = new JSONObject();
		jsonObject.put("data", json);
		//签名
		jsonObject.put("sign", Md5.md5Digest(json.toJSONString() + smac));
		String ciphertext = DESUtil.ENCRYPTMethod(jsonObject.toJSONString(), key).toUpperCase();
		System.out.println(ciphertext);

		decode(ciphertext);
	}
	
	// 出金接口数据加密方法
	public static void withdraw() throws Exception {
		JSONObject json = new JSONObject();
		//真实姓名
		json.put("realName", "实名02");
		//证件号码
		json.put("idCard", "2222222222");
		//币种id：1=BTC, 2=ETH, 3=UND
		json.put("coinId", "1");
		//出金指定币种对应币的数量
		json.put("amount", "0.48");
		//商家平台出金订单ID
		json.put("orderId", "2018071318275278925");
		//出金时间
		json.put("sendTime", "2017-08-30 22:05:31");
		//出金成功后，定时 同步Url
		json.put("notifyUrl", "https://www.domain.com/notify/withdraw/callback");
		//出金成功后，定时 同步Url
		json.put("returnUrl", "https://www.domain.com/return/withdraw/callback");
		
		JSONObject jsonObject = new JSONObject();
		jsonObject.put("data", json);
		//签名
		jsonObject.put("sign", Md5.md5Digest(json.toJSONString() + smac));
		String ciphertext = DESUtil.ENCRYPTMethod(jsonObject.toJSONString(), key).toUpperCase();
		System.out.println(ciphertext);

		decode(ciphertext);
	}
	
	
	public static void decode(String ciphertext) throws Exception {
		String data = DESUtil.decrypt(ciphertext, key);
		System.out.println(data);
	}
}
