# fexpay.co 平台接口

### 1. 客户入金
#### 基本信息

- 接口名称：客户入金
- 接口地址：http://fexpay.coinauc.com/uc/transfer/checkOutAppid 
- 请求方法：POST
- 请求数据类型：X-WWW-FORM-URLENCODED
- 响应类型：JSON
- 状态：有效
- 接口描述：入金时向OTC平台发起请求

#### 请求参数
| 参数名称  | 是否必须 | 数据类型 | 默认值 | 描述                     |
| --------- | -------- | -------- | ------ | ------------------------ |
| appid     | true     | string   |        | 商家在OTC平台ID                          |
| ciphertext| true     | string   |        | 加密内容 |
#### 响应参数
| 参数名称 | 是否必须 | 数据类型 | 描述                                    |
| -------- | -------- | -------- | --------------------------------------- |
|          | true     | string   | 返回一段 javascript，跳转到 OTC平台页面 |

#### ciphertext解密后的内容
| 参数名称  | 是否必须 | 数据类型 | 默认值 | 描述                     |
| --------- | -------- | -------- | ------ | ------------------------ |
| data      | true     | object   |        |                          |
| 　realName | false | string |      | 真实姓名         |
| 　amount    | true     | string   |        | 入金金额(美元)           |
| 　orderId   | true     | string   |        | 商家平台入金订单 ID      |
| 　idCard   | false | string |      | 身份证号         |
| 　notifyUrl | true     | string   |        | 入金成功后，定时 同步Url |
| 　returnUrl | true     | string   |        | 入金成功后，立即 调用Url |
| 　sendTime  | true     | string   |        | 入金时间                 |
| sign     | true  | string |      | 加密内容数字签名 |

例：
```
{
	"data": {
		"realName": "实名02",
		"amount": "5.25",
		"orderId": "2018071318275278924",
		"idCard": "2222222222",
		"notifyUrl": "https://www.domain.com/notify/callback",
		"returnUrl": "https://www.domain.com/return/callback",
		"sendTime": "2018-07-13 18:27:52"
	},
	"sign": "2AEB1CB86A15B970BA5BAB93E0E9D527"
}
```
注：JSON数据需按以上顺序生成

### 2. 客户入金数据定时上传 

#### 基本信息 

- 接口名称：客户入金数据定时上传 
- 接口地址：notifyUrl
- 请求方法：POST 
- 请求数据类型：X-WWW-FORM-URLENCODED 
- 响应类型：JSON 
- 状态：有效 
- 接口描述：调用URL，由「客户入金」参数notifyUrl指定，定时向收款商家平台发送入金订单数据。
- 返回值：
  - returnCode：由收款平台返回，1000表示成功。 

#### 请求参数

| 参数名称  | 是否必须 | 数据类型 | 默认值 | 描述                     |
| --------- | -------- | -------- | ------ | ------------------------ |
| appid     | true     | string   |        | 商家在OTC平台ID                          |
| ciphertext| true     | string   |        | 加密内容 |
#### 响应参数
| 参数名称 | 是否必须 | 数据类型 | 描述                                    |
| -------- | -------- | -------- | --------------------------------------- |
| returnCode | true     | int   | 1000 |
| returnMsg | true     | string   | 服务信息 |

#### ciphertext密文加密前数据格式 

| 参数名称   | 是否必须 | 数据类型 | 默认值 | 描述                                 |
| ---------- | -------- | -------- | ------ | ------------------------------------ |
| data       | true     | object   |        |                                      |
| 　orderId    | true     | string   |        | 入金订单ID                           |
| 　amount     | true     | string   |        | 入金金额                             |
| 　sendTime   | true     | string   |        | 请求上传时间                         |
| 　coin       | true     | string   |        | 虚拟币名称                           |
| 　coinRate   | true     | string   |        | 交易时币种市价                       |
| 　coinAmount | true     | string   |        | 交易币的数量                         |
| 　coinId     | true     | string   |        | OTC平台生成的订单ID 11位,如: E1012181377813 |
| 　code       | true     | string   |        | OTC平台处理码 (1000:成功)            |
| 　msg        | true     | string   |        | OTC平台处理消息                      |
| sign       | true     | string   |        | 加密字段签名                         |

例：
```
{
	"data": {
		"msg": "Success",
		"coinId": "E1012181377813",
		"amount": "1000.00000000",
		"coinAmount": "1000.00000000",
		"code": "1000",
		"orderId": "NOxxxxxxxxxxx",
		"coinRate": "1.00000000",
		"sendTime": "2018-10-12 15:35:27",
		"coin": "UND"
	},
	"sign": "AD50385C9129D896A5B29DE480B323BA"
}
```
注：JSON数据需按以上顺序生成

#### 响应参数 

| 参数名称   | 是否必须 | 数据类型 | 描述             |
| ---------- | -------- | -------- | ---------------- |
| returnCode | true     | int   | 商家平台返回结果 |
| returnMsg  | true     | string   | 商家平台返回信息 |

### 3. 预出金验证请求 

#### 基本信息 

- 接口名称：预出金验证请求 
- 接口地址：http://fexpay.coinauc.com/transfer/checkWithdrawal 
- 请求方法：POST
- 请求数据类型：X-WWW-FORM-URLENCODED
- 响应类型：JSON 
- 状态：有效 
- 接口描述：由收款平台发起，OTC平台验证认：
  1. 验证客户在OTC平台是否做过KYC认证
  2. 验证认证商家账户余额是否充足
- 参数：id_card，user_name用于OTC平台KYC查询。 
- 返回值code：
  - 0：表示成功
  - 4000 = 转账参数不全
  - 4001 = 商家不合法
  - 4002 = 商家没有转装权限
  - 4003 = 密文不合法
  - 4004 = 签名验证失败
  - 4005 = 获取市场价格失败
  - 4006 = 没有可用于支付的虚拟币
  - 4007 = 转装金额不合法
  - 4008 = 订单和用户不一致，需要重新登录
  - 4009 = 非法的订单
  - 4010 = 转账失败
  - 4011 = 币不足
  - 4012 = 身份验证失败
  - 4013 = 需要实名验证
  - 4014 = 钱包地址不合法
  - 4015 = 同一用户交易
  - 4016 = 订单类型不正确
  - 4017 = 批转账数据为空
  - 4018 = 批转账数据不合法
  - 4019 = 转账订单重复

#### 请求参数
| 参数名称  | 是否必须 | 数据类型 | 默认值 | 描述                     |
| --------- | -------- | -------- | ------ | ------------------------ |
| appid     | true     | string   |        | 商家在OTC平台ID                          |
| ciphertext| true     | string   |        | 加密内容 |

#### 响应参数 

| 参数名称 | 是否必须 | 数据类型 | 描述                                                         |
| -------- | -------- | -------- | -------------------------- |
| code     | true     | string   | OTC平台返回码 |
| message  | true     | string   | OTC平台返回信息   |

#### ciphertext加密前内容

| 参数名称      | 是否必须 | 数据类型 | 默认值 | 描述                             |
| ------------- | -------- | -------- | ------ | -------------------------------- |
| data          | true     | object   |        |                                  |
| 　idCard        | true     | string   |        | 出金人身份证号                   |
| 　coinId | true     | string   |        | 指定币种   1=BTC, 2=ETH, 3=UND|
| 　amount        | true     | string   |        | 出金指定币种对应 币的数量    |
| 　realName      | true     | string   |        | 出金人真实姓名                   |
| sign          | true     | string   |        | 加密内容签名                     |

例：
```
{
	"data": {
		"idCard": "xxxxxx",
		"coinId": "3",
		"amount": "41271.6",
		"realName": "default"
	},
	"sign": "3BE94D74A6039E61D93E653250218192"
}
```
注：JSON数据需按以上顺序生成

### 4. 客户出金 
#### 基本信息 

- 接口名称：客户出金 
- 接口地址：http://fexpay.coinauc.com/transfer/withdrawal 
- 请求方法：POST 
- 请求数据类型：X-WWW-FORM-URLENCODED 
- 响应类型：JSON 
- 状态：有效
- 接口描述：收款平台审核客户出金数据通过后，向OTC平台发出此请求。
- 参数：
  - idCard、realName 必填，OTC平台通idCard、realName对比，如不一致，返回1100。
- 返回值code：
  - 0：表示成功；
  - 4000 = 转账参数不全
  - 4001 = 商家不合法
  - 4002 = 商家没有转装权限
  - 4003 = 密文不合法
  - 4004 = 签名验证失败
  - 4005 = 获取市场价格失败
  - 4006 = 没有可用于支付的虚拟币
  - 4007 = 转装金额不合法
  - 4008 = 订单和用户不一致，需要重新登录
  - 4009 = 非法的订单
  - 4010 = 转账失败
  - 4011 = 币不足
  - 4012 = 身份验证失败
  - 4013 = 需要实名验证
  - 4014 = 钱包地址不合法
  - 4015 = 同一用户交易
  - 4016 = 订单类型不正确
  - 4017 = 批转账数据为空
  - 4018 = 批转账数据不合法
  - 4019 = 转账订单重复


#### 请求参数
| 参数名称  | 是否必须 | 数据类型 | 默认值 | 描述                     |
| --------- | -------- | -------- | ------ | ------------------------ |
| appid     | true     | string   |        | 商家在OTC平台ID                          |
| ciphertext| true     | string   |        | 加密内容 |
#### 响应参数 

| 参数名称 | 是否必须 | 数据类型 | 描述            |
| -------- | -------- | -------- | --------------- |
| code     | true     | string   | OTC平台返回码   |
| message  | true     | string   | OTC平台返回信息 |
#### ciphertext解密内容 

| 参数名称  | 是否必须 | 数据类型 | 默认值 | 描述                         |
| --------- | -------- | -------- | ------ | ---------------------------- |
| data      | true     | object   |        |                              |
| 　amount    | true     | string   |        | 出金指定币种对应币的数量 |
| 　notifyUrl | true     | string   |        | 出金成功后，定时同步URL     |
| 　returnUrl | true     | string   |        | 出金成功后，立即调用Url     |
| 　orderId   | true     | string   |        | 商家平台出金订单ID          |
| 　sendTime  | true     | string   |        | 出金时间                     |
| 　idCard    | true    | string   |        | 身份证                       |
| 　realName  | true    | string   |        | 真实姓名                     |
| 　coinId    | true     | string   |        | 1=BTC, 2=ETH, 3=UND          |
| sign      | true     | string   |        | 加密内容数字签名             |

### 5. 客户出金数据定时上传 

#### 基本信息 

- 接口名称：客户出金数据定时上传
- 接口地址：notifyUrl
- 请求方法：POST 
- 请求数据类型：X-WWW-FORM-URLENCODED
- 响应类型：JSON 
- 状态：有效 

- 接口描述：

#### 请求参数

| 参数名称  | 是否必须 | 数据类型 | 默认值 | 描述                     |
| --------- | -------- | -------- | ------ | ------------------------ |
| appid     | true     | string   |        | 商家在OTC平台ID                          |
| ciphertext| true     | string   |        | 加密内容 |

#### 响应参数 

| 参数名称   | 是否必须 | 数据类型 | 描述             |
| ---------- | -------- | -------- | ---------------- |
| returnCode | true     | string   | 商家平台返回Code |
| returnMsg  | true     | string   | 商家平台返回消息 |

#### ciphertext加密内容 

| 参数名称   | 是否必须 | 数据类型 | 默认值 | 描述                      |
| ---------- | -------- | -------- | ------ | ------------------------- |
| data       | true     | object   |        |                           |
| 　appId      | true     | string   |        | 商家在OTC平台ID           |
| 　orderId    | true     | string   |        | 出金订单ID                |
| 　sendTime   | true     | string   |        | 出金时间                  |
| 　coinId     | true     | string   |        | OTC平台上虚拟币 ID        |
| 　coinName   | true     | string   |        | OTC平台上虚拟币 名称      |
| 　coinRate   | true     | string   |        | 交易时币种市价            |
| 　coinAmount | true     | string   |        | 出金虚拟币的数量          |
| 　code       | true     | string   |        | OTC平台处理码 (1000:成功) |
| 　msg        | true     | string   |        | OTC平台处理消息           |
| sign       | true     | string   |        | 交互加密字段数据 签名     |

#### 测试账号：
#### 入/出金Fexpay商家账号：
- appid: bpwallet01
- Secret Key: 83A6AEDE-3B5A-4D3E-B789-DC780421C1A1
- smac: C628C57D3F0FCB7652D9C5D64898DFFF

#### 入/出金Fexpay客户账号：
- bpwallet02@gmail.com
- 登录密码：111111
- 资金密码：222222
- 真实姓名: 实名02
- 身份证号: 2222222222

### Leanwork接入
#### 测试环境
- 接口地址：http://fexpay.coinauc.com/transfer/leanwork/pay

##### 接入文档参见：《Leanwork租户自接支付通道接口》
