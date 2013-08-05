module Expression (calculate) where

    import Data.Char
    import Text.ParserCombinators.Parsec
    import Text.ParserCombinators.Parsec.Prim
    import Text.ParserCombinators.Parsec.Char
    import Text.ParserCombinators.Parsec.Combinator
    import Data.Number.BigFloat
    import Data.Number.Fixed
    import Data.Number.CReal
    import Data.Decimal
    
    type Number = CReal 

    data UnaryOperator =    
        Plus 
        | Minus 
        deriving (Eq, Show)
    
    data BinaryOperator =   
        Add 
        | Subtract 
        | Multiply 
        | Divide 
        | Exponentiation  
        deriving (Eq, Show)
    
    data Expression = 
        ConstantExpression Number
        | UnaryExpression Expression UnaryOperator
        | BinaryExpression Expression Expression BinaryOperator 
        deriving (Eq, Show)
        
    evalExpression :: Expression -> Number
    evalExpression (BinaryExpression left right Add) = (evalExpression left) + (evalExpression right)
    evalExpression (BinaryExpression left right Subtract) = (evalExpression left) - (evalExpression right)
    evalExpression (BinaryExpression left right Multiply) = (evalExpression left) * (evalExpression right)
    evalExpression (BinaryExpression left right Divide) = (evalExpression left) / (evalExpression right)
    evalExpression (BinaryExpression left right Exponentiation) = (evalExpression left) ** (evalExpression right)
    evalExpression (UnaryExpression operand Plus) = evalExpression operand
    evalExpression (UnaryExpression operand Minus) = -(evalExpression operand)
    evalExpression (ConstantExpression n) = n
        
    calculate :: String -> Number
    calculate input = 
        case parse parseExpression "" input of {
            (Left m) -> 0;
            (Right e) -> evalExpression e; }

    parseExpression :: GenParser Char st Expression
    parseExpression = do
        spaces
        value <- parseAdditiveExpression
        spaces
        return value
        
    parseAdditiveExpression :: GenParser Char st Expression
    parseAdditiveExpression = do
        spaces
        value <- parseMultiplicativeExpression
        spaces
        try (do {
            op <- oneOf "+-";
            spaces;
            result <- parseAdditiveExpression;
            spaces;
            return (BinaryExpression value result (case op of { '+' -> Add; '-' ->  Subtract; })); }) <|> return value
        
    parseMultiplicativeExpression :: GenParser Char st Expression
    parseMultiplicativeExpression = do
        spaces
        left <- parseUnaryExpression
        spaces
        try (do {
            op <- oneOf "*/^";
            spaces;
            right <- parseMultiplicativeExpression;
            spaces;
            return (BinaryExpression left right (case op of { '*' -> Multiply; '/' ->  Divide; '^' ->  Exponentiation; }))
        }) <|> return left
        
    parseUnaryExpression :: GenParser Char st Expression
    parseUnaryExpression = do
        spaces;
        op <- optionMaybe (oneOf "+-");
        spaces;
        value <- parsePrimaryExpression;
        let result = (case op of { 
                        Just '+' -> UnaryExpression value Plus; 
                        Just '-' -> UnaryExpression value Minus;
                        Nothing -> value });
        return result
        
    parsePrimaryExpression :: GenParser Char st Expression
    parsePrimaryExpression = do
        spaces;
        value <- parseConstantExpression <|> parseGroupingExpression
        return value 
            
    parseGroupingExpression :: GenParser Char st Expression
    parseGroupingExpression = do
        spaces;
        char '(';
        spaces;
        value <- parseExpression;
        spaces;
        char ')';
        return value;
        
    parseConstantExpression :: GenParser Char st Expression
    parseConstantExpression = do
        spaces
        ip <- parseIntegerPart;  
        fp <- parseFractionalPart <|> return 0.0; 
        ep <- parseExponentPart <|> return 1.0;
        let value = (ip + fp) * ep;
        return (ConstantExpression value);
        
    parseInteger :: [Char] -> Number
    parseInteger [] = 0
    parseInteger (c:cs) = 
        let csLen = (fromIntegral (length cs)) :: Number;
            mod = (10.0 ** csLen) :: Number; in
        (fromIntegral(digitToInt c) * (mod)) + parseInteger cs
        
    parseFractional :: [Char] -> Number
    parseFractional [] = 0
    parseFractional (c:cs) =  
        let len = fromIntegral (length cs) :: Number; 
            exponent = (len - 1) :: Number;
            csLen = exponent :: Number;
            mod = (10.0 ** csLen) :: Number; in
        fromIntegral(digitToInt c) / (mod) + parseFractional cs

    parseSign :: Maybe Char -> Number
    parseSign (Just '-') = -1 
    parseSign value = 1

    parseIntegerPart :: GenParser Char st Number
    parseIntegerPart = do
        digits <- many1 digit
        let integerPart = (parseInteger digits);
        return integerPart

    parseExponentPart :: GenParser Char st Number
    parseExponentPart = do
        oneOf "Ee"
        sign <- optionMaybe (oneOf "+-")
        digits <- many1 digit 
        
        let signModifier = parseSign sign;
            integerPart = (parseInteger digits);
            power = signModifier * integerPart;
            result = 10.0 ** power
        return result
        
    parseFractionalPart :: GenParser Char st Number
    parseFractionalPart = do
        char '.'
        digits <- many1 digit
        let n = parseInteger digits;
            pow = -(fromIntegral (length digits));
            result = n * (10.0 ** pow)
        return result 