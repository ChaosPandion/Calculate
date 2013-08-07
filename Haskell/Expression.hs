module Expression (parseExpression, evalExpression, getContext) where

    import Data.Char
    import Data.Map
    import Data.List (foldr, map, all)
    import Text.ParserCombinators.Parsec
    import Text.ParserCombinators.Parsec.Prim 
    import Text.ParserCombinators.Parsec.Char
    import Text.ParserCombinators.Parsec.Combinator
    import Text.ParserCombinators.Parsec.Error
    import Data.Number.CReal
    import Control.Applicative ((*>), (<*))
    
    type Number = CReal 

    data UnaryOperator =    
        Plus 
        | Minus 
        deriving (Eq)
        
    instance Show UnaryOperator where
        show Plus = "+"
        show Minus = "-"
        
    unaryOp '+' = Plus
    unaryOp '-' = Minus
    
    data BinaryOperator =   
        Add 
        | Subtract 
        | Multiply 
        | Divide 
        | Exponentiation  
        deriving (Eq)
        
    instance Show BinaryOperator where
        show Add = "+"
        show Subtract = "-"
        show Multiply = "*"
        show Divide = "/"
        show Exponentiation = "^"
        
    binaryOp '+' = Add
    binaryOp '-' = Subtract
    binaryOp '*' = Multiply
    binaryOp '/' = Divide
    binaryOp '^' = Exponentiation
    
    data Expression = 
        ConstantExpression Number
        | NameExpression String
        | CallExpression Expression [Expression]
        | UnaryExpression Expression UnaryOperator
        | BinaryExpression Expression Expression BinaryOperator 
        deriving (Eq, Show)
        
        
    data Context = 
        Context { functions :: Map String Computation }   

    instance Show Context where
        show x = "Context"
        
    data CallContext = 
        CallContext Context [Value]  
        deriving (Show)  
                
    type Computation = CallContext -> Result
    
                    
    data Value =
        NumberValue Number
        | FunctionValue Computation
                
    instance Show Value where
        show (NumberValue n) = show n
        show (FunctionValue x) = "function"
        
    data Result =
        Success Context Value 
        | Failure Context String 
        deriving (Show)
                    
    parseExpression input = 
        parse _parseExpression "" input        
        
    evalExpression input ctx = 
        case parseExpression input of
            (Left m) -> Failure ctx "Failure"
            (Right e) -> _evalExpression ctx e

    _pi (CallContext ctx []) = Success ctx (NumberValue pi) 
    
    _exp (CallContext ctx [ NumberValue x ]) = Success ctx (NumberValue (exp x)) 
    
    _sqrt (CallContext ctx [ NumberValue x ]) = Success ctx (NumberValue (sqrt x))
    
    _log (CallContext ctx [ NumberValue x ]) = Success ctx (NumberValue (log x))  
    
    _logBase (CallContext ctx [ NumberValue x, NumberValue y ]) = Success ctx (NumberValue (logBase x y))
    
    _sin (CallContext ctx [ NumberValue x ]) = Success ctx (NumberValue (sin x))
    
    _tan (CallContext ctx [ NumberValue x ]) = Success ctx (NumberValue (tan x))
    
    _cos (CallContext ctx [ NumberValue x ]) = Success ctx (NumberValue (cos x))  
    
    _asin (CallContext ctx [ NumberValue x ]) = Success ctx (NumberValue (asin x))
    
    _atan (CallContext ctx [ NumberValue x ]) = Success ctx (NumberValue (atan x))
    
    _acos (CallContext ctx [ NumberValue x ]) = Success ctx (NumberValue (acos x)) 
    
    _sinh (CallContext ctx [ NumberValue x ]) = Success ctx (NumberValue (sinh x))
    
    _tanh (CallContext ctx [ NumberValue x ]) = Success ctx (NumberValue (tanh x))
    
    _cosh (CallContext ctx [ NumberValue x ]) = Success ctx (NumberValue (cosh x))  
    
    _asinh (CallContext ctx [ NumberValue x ]) = Success ctx (NumberValue (asinh x))
    
    _atanh (CallContext ctx [ NumberValue x ]) = Success ctx (NumberValue (atanh x))
    
    _acosh (CallContext ctx [ NumberValue x ]) = Success ctx (NumberValue (acosh x))
    
    getContext = 
        Context { 
            functions = fromList [ 
               ("pi", _pi),
               ("exp", _exp),
               ("sqrt", _sqrt),
               ("log", _log),
               ("logBase", _logBase),
               ("sin", _sin),
               ("tan", _tan),
               ("cos", _cos),                                   
               ("asin", _asin),
               ("atan", _atan),
               ("acos", _acos),
               ("sinh", _sinh),
               ("tanh", _tanh),
               ("cosh", _cosh),
               ("asinh", _asinh),
               ("atanh", _atanh),
               ("acosh", _acosh) ]
        }
    
    evalArgs ctx [] = Right []
    evalArgs ctx (e:es) = 
        case _evalExpression ctx e of 
            Success _ v -> 
                case evalArgs ctx es of 
                    Left e -> Left e
                    Right vs -> Right $ v:vs
            Failure _ m -> Left m 
            
    attemptCall ctx args f =
        case evalArgs ctx args of
            Left m -> Failure ctx m
            Right a -> f (CallContext ctx a)
            
    _evalExpression ctx (CallExpression (NameExpression name) args) =
        case Data.Map.lookup name (functions ctx) of
            Just f -> attemptCall ctx args f
                
    _evalExpression ctx (BinaryExpression left right op) =
        let leftOperand = _evalExpression ctx left;
            rightOperand = _evalExpression ctx right;
        in case (leftOperand, rightOperand) of
            ((Success _ (NumberValue x)), (Success _ (NumberValue y))) ->
                case op of 
                    Add -> Success ctx (NumberValue (x + y))
                    Subtract -> Success ctx (NumberValue (x - y))
                    Multiply -> Success ctx (NumberValue (x * y))
                    Divide -> Success ctx (NumberValue (x / y))
                    Exponentiation -> Success ctx (NumberValue (x ** y))
                    
    _evalExpression ctx (UnaryExpression operand op) =
        case _evalExpression ctx operand of
            (Success _ (NumberValue x)) ->
                case op of 
                    Plus -> Success ctx (NumberValue (x))
                    Minus -> Success ctx (NumberValue (-x))
                    
    _evalExpression ctx (ConstantExpression n) =
        Success ctx (NumberValue n)
                        
    _parseExpression = ignoreWhiteSpace parseAdditiveExpression
    
    parseAdditiveExpression = parseBinaryExpression parseMultiplicativeExpression "+-" parseAdditiveExpression
        
    parseMultiplicativeExpression = parseBinaryExpression parseUnaryExpression "*/^" parseMultiplicativeExpression
    
    parseBinaryExpression leftParser ops rightParser = do
        left <- ignoreWhiteSpace leftParser
        op <- optionMaybe $ oneOf ops
        case op of
            Just c -> do
                right <- ignoreWhiteSpace rightParser
                return $ BinaryExpression left right (binaryOp c)
            Nothing -> do
                return left
                
    parseUnaryExpression = do
        op <- optionMaybe (ignoreWhiteSpace (oneOf "+-"))
        operand <- ignoreWhiteSpace (try (parseCallExpression) <|> parsePrimaryExpression);
        case op of
            Just c -> do
                return $ UnaryExpression operand (unaryOp c)
            Nothing -> do
                return operand
                
    parseCallExpression = do
        e <- parseNameExpression
        case e of 
            NameExpression _ -> do 
                args <- (many (try (space >> parseNameExpression))) <|> return []
                return (CallExpression e args)
            otherwise -> do 
                return e 
        
    parseNameExpression = do
        try (parsePrimaryExpression) 
            <|> do {        
                spaces;
                head <- letter; 
                tail <- many $ alphaNum;
                return $ NameExpression (head:tail); }
        
    parsePrimaryExpression = spaces *> parseConstantExpression <|> parseGroupingExpression
            
    parseGroupingExpression = wChar '(' *> _parseExpression <* wChar ')'
        
    parseConstantExpression = do
        spaces
        ip <- parseIntegerPart;  
        fp <- parseFractionalPart <|> return 0.0; 
        ep <- parseExponentPart <|> return 1.0;
        let value = (ip + fp) * ep;
        return (ConstantExpression value);
        
    parseInteger [] = 0
    parseInteger (c:cs) = 
        let csLen = (fromIntegral (length cs)) :: Number;
            mod = (10.0 ** csLen) :: Number; in
        (fromIntegral(digitToInt c) * (mod)) + parseInteger cs
        
    parseFractional [] = 0
    parseFractional (c:cs) =  
        let len = fromIntegral (length cs) :: Number; 
            exponent = (len - 1) :: Number;
            csLen = exponent :: Number;
            mod = (10.0 ** csLen) :: Number; in
        fromIntegral(digitToInt c) / (mod) + parseFractional cs

    parseSign (Just '-') = -1 
    parseSign value = 1

    parseIntegerPart = do
        digits <- many1 digit
        let integerPart = (parseInteger digits);
        return integerPart

    parseExponentPart = do
        oneOf "Ee"
        sign <- optionMaybe (oneOf "+-")
        digits <- many1 digit 
        
        let signModifier = parseSign sign;
            integerPart = (parseInteger digits);
            power = signModifier * integerPart;
            result = 10.0 ** power
        return result
        
    parseFractionalPart = do
        char '.'
        digits <- many1 digit
        let n = parseInteger digits;
            pow = -(fromIntegral (length digits));
            result = n * (10.0 ** pow)
        return result 
        
    ignoreWhiteSpace p = spaces *> p <* spaces 
    
    wChar c = ignoreWhiteSpace $ char c