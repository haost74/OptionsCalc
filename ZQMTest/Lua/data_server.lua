package.cpath = package.cpath .. ";" .. getScriptPath() .. [[\?51.dll]]
--require"QL"
require"zmq"
local json=require("dkjson")
--require"zhelpers"
is_run=false
itosend={}
postosend={}
acctosend={}
filter_acc=''
instruments={}
accounts={}
positions={}
positions_keys={}
is_connected=false
local sfind=string.find
FUT_OPT_CLASSES="FUTUX,SPBFUT,OPTUX,SPBOPT"


function OnParam(class,sec)
	if is_run and is_connected and (class=='SPBFUT' or class=='FUTUX' or class=='OPTUX' or class=='SPBOPT') then
		--local st=os.clock()
		-- or class=='OPTUX' class=='SPBOPT' or 
		local i=instruments[sec].Dynamic
		i.LastPrice=tonumber(getParamEx(class,sec,"Last").param_value)
		i.Volatility=tonumber(getParamEx(class,sec,"Volatility").param_value)
		i.TheorPrice=tonumber(getParamEx(class,sec,"theorprice").param_value)
		i.Bid=tonumber(getParamEx(class,sec,'BID').param_value)
		i.Ask=tonumber(getParamEx(class,sec,'OFFER').param_value)
		i.BidVol=tonumber(getParamEx(class,sec,'BIDDEPTH').param_value)
		i.AskVol=tonumber(getParamEx(class,sec,'OFFERDEPTH').param_value)
		itosend[#itosend+1]=json.encode(i)
		--[[if instruments[sec]==nil then message("nil "..sec,3) return end
		--if pr~=i.LastPrice or volat~=i.Volatility or theorpr~=i.TheorPrice then
			--table.insert(tosend,tostring(sec.."="..pr))
			
			--message('sended',1)
			--publisher:send(sec..' Last='..pr)
			--..' Volat='..volat..' TheorPrice='..theorpr
			i.LastPrice=pr
			i.Volatility=volat
			i.TheorPrice=theorpr
		end
		]]
		--message("time="..(os.clock()-st),3)
	end
end

function OnFuturesClientHolding(hold)
	if is_run and is_connected and hold~=nil and (filter_acc=='' or string.find(filter_acc,hold.trdaccid)~=nil) then
		--toLog(log,'New holding update')
		--table.insert(acctosend,jsonhold)
		local t=positions[positions_keys[hold.trdaccid..hold.sec_code]]
		t.TotalNet=hold.totalnet
		postosend[#postosend+1]=json.encode(t)
	end
end

function OnStop()
	is_run=false
	--publisher:close()
	--context:term()
end

function OnInitDo()
	--context=zmq.init(1)
	--publisher=context:socket(zmq.PUB)
	--publisher:bind("tcp://127.0.0.1:5563")
	local id=1
	for cl in string.gmatch(FUT_OPT_CLASSES,"%a+") do
		local sec_list=getClassSecurities(cl)
		for sec in string.gmatch(sec_list,"%w+%.?%w+") do
			instruments[sec]={}
			instruments[sec].Static={}
			instruments[sec].Dynamic={}
			local static=instruments[sec].Static
			local dynamic=instruments[sec].Dynamic
			static.Class=cl
			static.Code=sec
			static.FullName=getParamEx(cl,sec,'LONGNAME').param_image
			static.Id=id
			if cl=='FUTUX' or cl=='SPBFUT' then static.InstrumentType='Futures' else static.InstrumentType='Option' end

			static.OptionType=getParamEx(cl,sec,"OPTIONTYPE").param_image
			static.Strike=tonumber(getParamEx(cl,sec,"STRIKE").param_value)
			static.BaseContract=getParamEx(cl,sec,"OPTIONBASE").param_image
			static.DaysToMate=getParamEx(cl,sec,"DAYS_TO_MAT_DATE").param_image
			static.MaturityDate=getParamEx(cl,sec,"MAT_DATE").param_image
			
			dynamic.LastPrice=tonumber(getParamEx(cl,sec,'last').param_value)
			dynamic.Volatility=tonumber(getParamEx(cl,sec,'volatility').param_value)
			dynamic.TheorPrice=tonumber(getParamEx(cl,sec,'theorprice').param_value)
			dynamic.Bid=tonumber(getParamEx(cl,sec,'BID').param_value)
			dynamic.Ask=tonumber(getParamEx(cl,sec,'OFFER').param_value)
			dynamic.BidVol=tonumber(getParamEx(cl,sec,'BIDDEPTH').param_value)
			dynamic.AskVol=tonumber(getParamEx(cl,sec,'OFFERDEPTH').param_value)
			dynamic.Id=id
			--dynamic.MsgType='INSTRUMENT'
			id=id+1
		end
	end
	local sf=string.find
	id=1
	for i=1,getNumberOf('trade_accounts') do
		local itm=getItem('trade_accounts',i)
		if (sf(itm.class_codes,'FUTUX')~=nil or sf(itm.class_codes,'OPTUX')~=nil or sf(itm.class_codes,'SPBFUT')~=nil or sf(itm.class_codes,'SPBOPT')~=nil ) then
			accounts[#accounts+1]={['Name']=itm.trdaccid,['Id']=id}
			--accounts[#accounts].Name=itm.trdaccid
			--accounts[#accounts].Id=id
			id=id+1
		end
	end
	for i=1,getNumberOf('futures_client_holding') do
		local itm=getItem('futures_client_holding')
		positions[#positions+1]={
			['AccountName']=itm.trdaccid,
			['SecurityCode']=itm.sec_code,
			['TotalNet']=itm.totalnet
		}
		positions_keys[itm.trdaccid..itm.sec_code]=#positions
	end
	return true
	--is_run=true
end
function OnConnected(rep,pub)
	--reply:recv()
	message('Connected',3)
	rep:send('CONNECTED')
	for k,v in pairs(instruments) do
		pub:send('NEWINSTRUMENT',zmq.SNDMORE)
		pub:send(json.encode(v.Static))

		pub:send('INSTRUMENT',zmq.SNDMORE)
		pub:send(json.encode(v.Dynamic))

	end
	for k,v in ipairs(accounts) do
		pub:send('ACCOUNT',zmq.SNDMORE)
		pub:send(json.encode(v))
	end
	for k,v in ipairs(positions) do
		pub:send('POSITION',zmq.SNDMORE)
		pub:send(json.encode(v))
	end

	pub:send('COMMON',zmq.SNDMORE)
	pub:send('INITIALSYNCEND')
	message('INITIALSYNCEND',3)
	return true
end
function main()
	
	is_run=OnInitDo()
	local context=zmq.init(1)
	local publisher=context:socket(zmq.PUB)
	local reply=context:socket(zmq.REP)
	publisher:bind("tcp://127.0.0.1:5563")
	reply:bind("tcp://127.0.0.1:5562")
	
	while is_run do
		msg=reply:recv(zmq.NOBLOCK)
		--message(tostring(msg),1)
		if msg~=nil then
			message('start onconnect',3)
			is_connected=OnConnected(reply,publisher)
			message('end onconnect '..tostring(is_connected),3)
		end
		if #itosend~=0 then
			for i=1,#itosend do
				local msg=table.remove(itosend,i)
				if msg~=nil then
					publisher:send("INSTRUMENT",zmq.SNDMORE)
					res=publisher:send(msg)
				end
			end
			--message("#"..#tosend,)2zmq
		end
		if #postosend~=0 then
			for i=1,#postosend do
				local msg=table.remove(postosend,i)
				if msg~=nil then
					publisher:send("POSITION",zmq.SNDMORE)
					res=publisher:send(msg)
				end
			end
			--message("#"..#tosend,2)
		end
		sleep (1)
	end
	publisher:close()
	reply:close()
	context:term()
	
	--while is_run do sleep(100) end
end

